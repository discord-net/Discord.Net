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

    private readonly Dictionary<Type, IStoreInfo> _rootStores = [];
    private readonly KeyedSemaphoreSlim<Type> _rootStoreSemaphores = new(1, 1);


    private readonly Dictionary<Type, IEntityBroker> _brokers = [];
    private readonly KeyedSemaphoreSlim<Type> _brokerSemaphores = new(1, 1);

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
        where TEntity : ICacheableEntity
        => CanUseStoreType(typeof(TEntity));

    public bool CanUseStoreType(Type type)
        => _client.Config.CreateStoreForEveryEntity || _client.Config.ExtendedStoreTypes.Contains(type);

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

    public TEntity CreateLatent<TId, TEntity, [TransitiveFill] TActor, TModel>(TActor actor, TModel model,
        CachePathable? path = null)
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
        if (TryGetCachedBroker<TId, TEntity, TModel>(out var broker))
            return CreateLatentFromBroker(broker, path ?? CachePathable.Empty, model, actor);

        // TODO: sync lock here, I don't like it
        using var scope = _brokerSemaphores.Get(typeof(TEntity), out var semaphoreSlim);

        semaphoreSlim.Wait(_backgroundTokenSource.Token);

        try
        {
            if (TryGetCachedBroker(out broker))
                return CreateLatentFromBroker(broker, path ?? CachePathable.Empty, model, actor);

            broker = new EntityBroker<TId, TEntity, TActor, TModel>(_client, this);
            _brokers[typeof(TEntity)] = broker;

            return CreateLatentFromBroker(broker, path ?? CachePathable.Empty, model, actor);
        }
        finally
        {
            semaphoreSlim.Release();
        }
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

        var latentEntity = TEntity.Construct(_client, context, model);

        _operationChannel.Writer.TryWrite(
            new AttachLatentEntityOperation<TId, TEntity, TModel>(latentEntity, model, handle, broker)
        );

        return latentEntity;
    }

    public async ValueTask<IEntityBroker<TId, TEntity, TActor, TModel>> GetBrokerAsync<
        TId,
        TEntity,
        TActor,
        TModel
    >(
        CancellationToken token = default
    )
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
        if (TryGetCachedBroker<TId, TEntity, TActor, TModel>(out var broker))
            return broker;

        using var scope = _brokerSemaphores.Get(typeof(TEntity), out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (TryGetCachedBroker(out broker))
                return broker;

            broker = new EntityBroker<TId, TEntity, TActor, TModel>(_client, this);
            _brokers[typeof(TEntity)] = broker;
            return broker;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private bool TryGetCachedBroker<TId, TEntity, TModel>(
        [MaybeNullWhen(false)] out IEntityBroker<TId, TEntity, TModel> broker)
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        broker = null;

        if (_brokers.TryGetValue(typeof(TEntity), out var cachedBroker) &&
            cachedBroker is IEntityBroker<TId, TEntity, TModel> value)
        {
            broker = value;
        }

        return broker is not null;
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

    public ValueTask<IStoreInfo<TId, TModel>> GetRootStoreAsync<[TransitiveFill]TProvider, TId, TEntity, TModel>(
        Template<TProvider> template,
        CancellationToken token = default)
        where TProvider :
        IRootStoreProvider<TId, TModel>, IActor<TId, TEntity>
        where TEntity : class, IEntityOf<TModel>, IEntity<TId>
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
        => GetRootStoreAsync<TProvider, TId, TModel>(token);

    public async ValueTask<IStoreInfo<TId, TModel>> GetRootStoreAsync<TProvider, TId, TModel>(
        CancellationToken token = default
    )
        where TProvider : IRootStoreProvider<TId, TModel>
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        if (TryGetRootStore<TModel>(out var store))
        {
            if (store is IStoreInfo<TId, TModel> storeInfo)
                return storeInfo;

            _logger.LogWarning(
                "Unexpected root store info found: Expecting {Model}, but got {Actual}",
                typeof(TModel),
                store.ModelType
            );

            lock(_rootStoreSemaphores)
                _rootStores.Remove(typeof(TModel));
        }

        using var scope = _rootStoreSemaphores.Get(typeof(TModel), out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            var info = (await TProvider.GetStoreAsync(_client, CachePathable.Empty, token))
                .ToInfo(_client, Template.Of<TProvider>());

            lock (_rootStoreSemaphores)
                _rootStores[typeof(TModel)] = info;

            return info;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private bool TryGetRootStore<TModel>([MaybeNullWhen(false)] out IStoreInfo info)
    {
        lock (_rootStoreSemaphores)
            return _rootStores.TryGetValue(typeof(TModel), out info);
    }

    public void Dispose()
    {
        _controllerBackgroundTask?.Dispose();
        _backgroundTokenSource.Dispose();
    }
}
