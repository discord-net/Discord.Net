using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Discord.Gateway;
using Discord.Gateway.State.Operations;
using Discord.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Discord.Gateway.State;

internal sealed partial class StateController(
    DiscordGatewayClient client,
    ILogger<StateController> logger
) : 
    IDisposable
{
    internal MutableSelfUserModel? SelfUserModel { get; set; }

    private Task? _controllerBackgroundTask;
    private CancellationTokenSource _backgroundTokenSource = new();

    private readonly ILogger<StateController> _logger = logger;

    private readonly Dictionary<Type, IEntityBroker> _brokers = new();
    private readonly object _brokersSyncRoot = new();

    private readonly Channel<IStateOperation> _operationChannel = Channel.CreateUnbounded<IStateOperation>(
        new UnboundedChannelOptions {SingleReader = true, AllowSynchronousContinuations = false}
    );

    public async ValueTask StartBackgroundProcessing(CancellationToken token)
    {
        _backgroundTokenSource.Cancel();

        if (_controllerBackgroundTask is not null)
        {
            await _controllerBackgroundTask;
            _controllerBackgroundTask.Dispose();
        }

        _backgroundTokenSource.Dispose();
        _backgroundTokenSource = new();

        _backgroundTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        _controllerBackgroundTask = RunBackgroundProcessingAsync(
            _backgroundTokenSource.Token
        );
    }

    public bool CanUseStoreType<TEntity>()
        where TEntity : ICacheableEntity
        => CanUseStoreType(typeof(TEntity));

    public bool CanUseStoreType(Type type)
        => client.Config.CreateStoreForEveryEntity || client.Config.ExtendedStoreTypes.Contains(type);

    private async Task RunBackgroundProcessingAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var operation = await _operationChannel.Reader.ReadAsync(token);

            switch (operation)
            {
                case CleanupOperation cleanup:
                    await cleanup.CleanupTask(token);
                    break;
                case IAttachLatentEntityOperation latent:
                    await latent.AttachAsync(this, token);
                    break;
                case IUpdateOperation update:
                    await update.UpdateAsync(token);
                    break;
            }

            if (operation is IDisposable disposable)
                disposable.Dispose();
        }
    }

    public TEntity CreateLatent<TId, TEntity, [TransitiveFill] TActor, TModel>(
        TActor actor,
        TModel model,
        CachePathable? path = null
    )
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreInfoProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
        where TActor :
        class,
        IPathable,
        IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        => CreateLatent<TId, TEntity, TActor, TModel>(model, path ?? new() {actor}, actor);

    public TEntity CreateLatent<TId, TEntity, TActor, TModel>(
        TModel model,
        CachePathable? path = null,
        TActor? actor = null
    )
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreInfoProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
        where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        return CreateLatentFromBroker(
            GetBroker<TId, TEntity, TActor, TModel>(),
            path ?? CachePathable.Empty,
            model,
            actor
        );
    }

    private TEntity CreateLatentFromBroker<TId, TEntity, TModel, TActor>(
        IEntityBroker<TId, TEntity, TModel> broker,
        CachePathable path,
        TModel model,
        TActor? actor
    )
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreInfoProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IActor<TId, TEntity>
    {
        if (
            !broker.TryCreateLatentHandle(
                model,
                out var entity,
                out var handle,
                _backgroundTokenSource.Token
            )
        )
        {
            _operationChannel.Writer.TryWrite(
                new UpdateOperation<TId, TEntity, TModel>(entity, model, broker)
            );

            return entity;
        }

        IGatewayConstructionContext context = actor is not null
            ? new GatewayConstructionContext<TActor>(actor, path)
            : new GatewayConstructionContext(path);

        var latentEntity = TEntity.Construct(client, context, model);

        _operationChannel.Writer.TryWrite(
            new AttachLatentEntityOperation<TId, TEntity, TModel>(latentEntity, model, handle, broker)
        );

        return latentEntity;
    }

    public IEntityBroker<TId, TEntity, TActor, TModel> GetBroker<TId, TEntity, TActor, TModel>()
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
        where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        lock (_brokersSyncRoot)
        {
            if (TryGetCachedBroker<TId, TEntity, TActor, TModel>(out var broker))
                return broker;

            broker = new EntityBroker<TId, TEntity, TActor, TModel>(
                client,
                this
            );
            _brokers[typeof(TEntity)] = broker;
            return broker;
        }
    }

    private bool TryGetCachedBroker<TId, TEntity, TActor, TModel>(
        [MaybeNullWhen(false)] out IEntityBroker<TId, TEntity, TActor, TModel> broker)
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
        where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        broker = null;

        if (_brokers.TryGetValue(typeof(TEntity), out var cachedBroker) &&
            cachedBroker is IEntityBroker<TId, TEntity, TActor, TModel> value)
        {
            broker = value;
        }

        return broker is not null;
    }

    public void Dispose()
    {
        _controllerBackgroundTask?.Dispose();
        _backgroundTokenSource.Dispose();
    }
}