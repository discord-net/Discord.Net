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

internal sealed partial class StateController : IDisposable
{
    internal MutableSelfUserModel? SelfUserModel { get; set; }

    private ICacheProvider CacheProvider => _client.CacheProvider;

    private Task? _controllerBackgroundTask;
    private CancellationTokenSource _backgroundTokenSource;

    private readonly DiscordGatewayClient _client;
    private readonly ILogger<StateController> _logger;

    private readonly Dictionary<Type, IRefCounted<IEntityBroker>> _brokers = [];
    private readonly KeyedSemaphoreSlim<Type> _brokerSemaphore = new(1, 1);

    private readonly Channel<IStateOperation> _operationChannel = Channel.CreateUnbounded<IStateOperation>(
        new UnboundedChannelOptions {SingleReader = true, AllowSynchronousContinuations = false}
    );

    public StateController(
        DiscordGatewayClient client,
        ILogger<StateController> logger)
    {
        _client = client;
        _logger = logger;
        _backgroundTokenSource = new();
    }

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
        => _client.Config.CreateStoreForEveryEntity || _client.Config.ExtendedStoreTypes.Contains(typeof(TEntity));


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

    public TEntity CreateLatent<TId, TEntity, [TransitiveFill] TActor, TModel>(TActor actor, TModel model)
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
        where TActor : class, IPathable, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        => CreateLatent<TId, TEntity, TActor, TModel>(actor, actor, model);

    public TEntity CreateLatent<TId, TEntity, TActor, TModel>(TActor actor, IPathable path, TModel model)
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
        where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        if (TryGetCachedBroker<TId, TEntity, TModel>(out var broker))
            return CreateLatentFromBroker(broker, path, model);

        // TODO: sync lock here, I don't like it
        using var scope = _brokerSemaphore.Get(typeof(TEntity), out var semaphoreSlim);

        semaphoreSlim.Wait(_backgroundTokenSource.Token);

        try
        {
            if (TryGetCachedBroker(out broker))
                return CreateLatentFromBroker(broker, path, model);

            broker = new RefCounted<IEntityBroker<TId, TEntity, TModel>>(
                new EntityBroker<TId, TEntity, TActor, TModel>(_client, this)
            );
            _brokers[typeof(TEntity)] = broker;

            return CreateLatentFromBroker(broker, path, model);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private TEntity CreateLatentFromBroker<TId, TEntity, TModel>(
        IRefCounted<IEntityBroker<TId, TEntity, TModel>> broker,
        IPathable path,
        TModel model
    )
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        if (!broker.Value.TryCreateLatentHandle(model, out var entity, out var handle,
                _backgroundTokenSource.Token))
        {
            _operationChannel.Writer.TryWrite(
                new UpdateOperation<TId, TEntity, TModel>(entity, model, broker)
            );

            return entity;
        }

        var latentEntity = TEntity.Construct(_client, new CacheConstructionContext<TId, TEntity>(path), model);

        _operationChannel.Writer.TryWrite(
            new AttachLatentEntityOperation<TId, TEntity, TModel>(latentEntity, model, handle, broker)
        );

        return latentEntity;
    }

    public void EnqueueEntityDestruction<TId, TEntity, TModel>(TId id)
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        if (!_brokers.TryGetValue(typeof(TEntity), out var broker) || !broker.HasValue)
            return;

        if (broker.Value is not IEntityBroker<TId, TEntity, TModel> entityBroker)
            return;

        _operationChannel.Writer.TryWrite(
            new CleanupOperation(token => entityBroker.DestroyReferenceAsync(id, token))
        );
    }

    [MustDisposeResource]
    public async ValueTask<IRefCounted<IEntityBroker<TId, TEntity, TActor, TModel>>> GetBrokerAsync<TId, TEntity, TActor, TModel>(
        CancellationToken token = default
    )
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
        where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        if (TryGetCachedBroker<TId, TEntity, TActor, TModel>(out var broker))
            return broker;

        using var scope = _brokerSemaphore.Get(typeof(TEntity), out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (TryGetCachedBroker(out broker))
                return broker;

            broker = new RefCounted<IEntityBroker<TId, TEntity, TActor, TModel>>(
                new EntityBroker<TId, TEntity, TActor, TModel>(_client, this)
            );
            _brokers[typeof(TEntity)] = broker;
            return broker;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private bool TryGetCachedBroker<TId, TEntity, TModel>(
        [MaybeNullWhen(false)] out IRefCounted<IEntityBroker<TId, TEntity, TModel>> broker)
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        broker = null;

        if (_brokers.TryGetValue(typeof(TEntity), out var cachedBroker) &&
            cachedBroker is RefCounted<IEntityBroker<TId, TEntity, TModel>> value)
        {
            value.AddReference();
            broker = value;
        }

        return broker is not null;
    }

    private bool TryGetCachedBroker<TId, TEntity, TActor, TModel>(
        [MaybeNullWhen(false)] out IRefCounted<IEntityBroker<TId, TEntity, TActor, TModel>> broker)
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
        where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        broker = null;

        if (_brokers.TryGetValue(typeof(TEntity), out var cachedBroker) &&
            cachedBroker is RefCounted<IEntityBroker<TId, TEntity, TActor, TModel>> value)
        {
            value.AddReference();
            broker = value;
        }

        return broker is not null;
    }

    public async ValueTask<IEntityModelStore<TId, TModel>> GetStoreAsync<
        [TransitiveFill] TIdentity,
        TId,
        TEntity,
        TActor,
        TModel
    >(
        Template<TIdentity> template,
        CancellationToken token = default
    )
        where TIdentity : IIdentifiable<TId, TEntity, TActor, TModel>
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TActor : class, IActor<TId, TEntity>
        where TModel : class, IEntityModel<TId>
    {
        return await CacheProvider.GetStoreAsync<TId, TModel>(token);
    }

    public void Dispose()
    {
        _controllerBackgroundTask?.Dispose();
        _backgroundTokenSource.Dispose();
    }
}
