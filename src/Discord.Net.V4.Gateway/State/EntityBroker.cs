using Discord.Gateway;
using Discord.Gateway.State.Handles;
using Discord.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway.State;

internal sealed class EntityBroker<TId, TEntity, TActor, TModel> : IEntityBroker<TId, TEntity, TActor, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
    where TModel : class, IEntityModel<TId>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
    where TId : IEquatable<TId>
{
    private sealed class LatentEntityPromise(IDisposable scope, SemaphoreSlim semaphore) : IDisposable
    {
        public void Dispose()
        {
            scope.Dispose();
            semaphore.Release();
        }
    }

    private sealed class EntityReference(
        EntityBroker<TId, TEntity, TActor, TModel> broker,
        TId id,
        TEntity entity,
        params IEntityHandle<TId, TEntity>[] handles
    )
    {
        public bool IsAlive => !_isKilled && (Handles.Count > 0 || WeakReference.TryGetTarget(out _));

        private bool _isKilled = false;

        public TId Id { get; } = id;

        public WeakReference<TEntity> WeakReference { get; private set; } = new(entity);

        public List<IEntityHandle<TId, TEntity>> Handles { get; private set; } = handles.ToList();

        public IEntityHandle<TId, TEntity>? AllocateHandle()
        {
            if (!WeakReference.TryGetTarget(out var entity))
                return null;

            var handle = new EntityHandle<TId, TEntity>(broker, Id, entity);
            Handles.Add(handle);
            return handle;
        }

        public ValueTask DestroyAsync()
        {
            if (_isKilled)
                return ValueTask.CompletedTask;

            _isKilled = true;

            if (Handles.Count > 0)
            {
                // TODO:
                // destroy is called when the underlying entity is killed, generally if theres any
                // handles remaining they are now in a faulty state. I need to investigate and figure
                // out if this is an error or if we can clear them in a dx friendly way

                Handles.Clear();
            }

            WeakReference = null!;
            Handles = null!;

            return ValueTask.CompletedTask;
        }
    }

    private readonly DiscordGatewayClient _client;
    private readonly StateController _controller;
    private readonly Dictionary<TId, EntityReference> _references;
    private readonly KeyedSemaphoreSlim<TId> _keyedSemaphore;
    private readonly SemaphoreSlim _enumerationSemaphore = new(1, 1);

    private readonly ILogger<EntityBroker<TId, TEntity, TActor, TModel>> _logger;

    public EntityBroker(
        DiscordGatewayClient client,
        StateController stateController)
    {
        _keyedSemaphore = new(1, 1);
        _client = client;
        _controller = stateController;
        _logger = client.LoggerFactory.CreateLogger<EntityBroker<TId, TEntity, TActor, TModel>>();
        _references = new();
    }

    private void InterceptSelfUser(ref TModel model)
    {
        if (model is not IUserModel userModel)
            return;

        if (userModel.Id != _client.CurrentUser.Id)
            return;

        if (userModel is ISelfUserModel selfUserModel)
        {
            if (_controller.SelfUserModel is null)
                _controller.SelfUserModel = new(selfUserModel);
            else
            {
                _controller.SelfUserModel.SelfUserModelPart = selfUserModel;
                _controller.SelfUserModel.UserModelPart = selfUserModel;
            }
        }
        else
        {
            if (_controller.SelfUserModel is null)
                return;
        }

        _controller.SelfUserModel.UserModelPart = userModel;

        model = (_controller.SelfUserModel as TModel)!;
    }

    public bool TryCreateLatentHandle(
        TModel model,
        [MaybeNullWhen(true)] out TEntity entity,
        [MaybeNullWhen(false)] out IDisposable handle,
        CancellationToken token)
    {
        var scope = _keyedSemaphore.Get(model.Id, out var semaphoreSlim);

        semaphoreSlim.Wait(token);

        if (_references.TryGetValue(model.Id, out var reference) && reference.WeakReference.TryGetTarget(out entity))
        {
            semaphoreSlim.Release();
            scope.Dispose();
            handle = null;
            return false;
        }

        handle = new LatentEntityPromise(scope, semaphoreSlim);
        entity = null;
        return true;
    }

    public bool TryGetInScope(TId id, [MaybeNullWhen(false)] out TEntity entity)
    {
        if (_references.TryGetValue(id, out var reference))
        {
            if (reference.WeakReference.TryGetTarget(out var target))
            {
                entity = target;
                return true;
            }

            RemoveDeadReference(reference);
        }

        entity = null;
        return false;
    }

    private void RemoveDeadReference(EntityReference reference)
    {
        if (reference.IsAlive) return;

        _references.Remove(reference.Id);
    }

    private EntityHandle<TId, TEntity> CreateReferenceAndHandle(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        TModel model,
        TActor? actor = null)
    {
        ICacheConstructionContext<TId, TEntity> context = actor is not null
            ? new CacheConstructionContext<TId, TEntity, TActor>(actor, path)
            : new CacheConstructionContext<TId, TEntity>(path);

        var entity = TEntity.Construct(_client, context, model);

        var handle = new EntityHandle<TId, TEntity>(this, identity.Id, entity);

        _references.Add(identity.Id, new EntityReference(this, identity.Id, entity, handle));

        return handle;
    }

    private TEntity CreateReferenceAndImplicitHandle(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        TModel model,
        TActor? actor = null)
    {
        var handle = new ImplicitHandle<TId, TEntity, TActor, TModel>(
            _client,
            this,
            path,
            identity.Id,
            model,
            actor,
            out var entity
        );

        _references.Add(identity.Id, new EntityReference(this, identity.Id, entity, handle));

        return entity;
    }

    public async Task DestroyReferenceAsync(TId id, CancellationToken token)
    {
        if (_references.TryGetValue(id, out var reference))
        {
            await reference.DestroyAsync();

            _references.Remove(id);
        }
    }

    public ValueTask ReleaseHandleAsync(IEntityHandle<TId, TEntity> handle)
    {
        if (!_references.TryGetValue(handle.Id, out var reference))
        {
            // TODO: this is a warning state
            return ValueTask.CompletedTask;
        }

        if (!reference.Handles.Remove(handle))
        {
            // TODO: this is a warning state
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask AttachLatentEntityAsync(TId id, TEntity entity, IEntityModelStore<TId, TModel> store,
        CancellationToken token)
    {
        using var scope = _keyedSemaphore.Get(id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (_references.TryGetValue(id, out var reference) && reference.IsAlive)
            {
                // TODO: this is an error state, mainly caused by a race condition
                // in the time that it took from constructing 'entity' and then registering it,
                // the state controller constructed another form of the entity. We can either
                // lock out this semaphore at construction time or we can allow multiple
                // references.
                return;
            }

            var model = entity.GetModel();

            InterceptSelfUser(ref model);

            await store.AddOrUpdateAsync(model, token);

            _references[id] = new(this, id, entity);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async ValueTask UpdateAsync(TModel model, IEntityModelStore<TId, TModel> store, CancellationToken token)
    {
        InterceptSelfUser(ref model);

        await store.AddOrUpdateAsync(model, token);

        if (_references.TryGetValue(model.Id, out var reference))
        {
            if (!reference.WeakReference.TryGetTarget(out var entity))
            {
                RemoveDeadReference(reference);
                return;
            }

            await entity.UpdateAsync(model, false, token);
        }
    }

    public async ValueTask<TEntity?> GetImplicitAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IEntityModelStore<TId, TModel> store,
        TActor? actor = null,
        CancellationToken token = default)
    {
        using var scope = _keyedSemaphore.Get(identity.Id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (_references.TryGetValue(identity.Id, out var reference))
            {
                // we can return the weak reference here, basically referencing it again.
                // this is a performance issue though, the callee becomes responsible for cleanup of the entity via GC
                if (reference.WeakReference.TryGetTarget(out var entity))
                    return entity;

                RemoveDeadReference(reference);
            }

            var model = await store.GetAsync(identity.Id, token);

            if (model is null)
                return null;

            return CreateReferenceAndImplicitHandle(path, identity, model, actor);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IEntityModelStore<TId, TModel> store,
        TActor? actor = null,
        CancellationToken token = default)
    {
        using var scope = _keyedSemaphore.Get(identity.Id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (_references.TryGetValue(identity.Id, out var reference))
            {
                var handle = reference.AllocateHandle();

                if (handle is not null)
                    return handle;

                // if we couldn't allocate a handle, remove the dead reference
                RemoveDeadReference(reference);
            }

            // fetch from the cache
            var model = await store.GetAsync(identity.Id, token);

            if (model is null)
                return null;

            return CreateReferenceAndHandle(path, identity, model, actor);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }


    public async ValueTask<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> GetAllHandlesAsync(
        CachePathable path,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token = default)
    {
        await _enumerationSemaphore.WaitAsync(token);

        var result = new List<IEntityHandle<TId, TEntity>>();

        try
        {
            await foreach (var model in store.GetAllAsync(token))
            {
                if (_references.TryGetValue(model.Id, out var reference))
                {
                    var handle = reference.AllocateHandle();

                    if (handle is null)
                    {
                        RemoveDeadReference(reference);
                        goto construct_entity;
                    }

                    await handle.Entity.UpdateAsync(model, false, token);

                    result.Add(handle);
                    continue;
                }

                construct_entity:
                result.Add(CreateReferenceAndHandle(path, IIdentifiable<TId, TEntity, TModel>.Of(model.Id), model));
            }

            return result.AsReadOnly();
        }
        finally
        {
            _enumerationSemaphore.Release();
        }
    }

    public async ValueTask<IReadOnlyCollection<TEntity>> GetAllImplicitAsync(
        CachePathable path,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token = default)
    {
        await _enumerationSemaphore.WaitAsync(token);

        var result = new List<TEntity>();

        try
        {
            await foreach (var model in store.GetAllAsync(token))
            {
                if (_references.TryGetValue(model.Id, out var reference))
                {
                    if (reference.WeakReference.TryGetTarget(out var entity))
                    {
                        result.Add(entity);
                        continue;
                    }

                    RemoveDeadReference(reference);
                }

                result.Add(CreateReferenceAndImplicitHandle(path, IIdentifiable<TId, TEntity, TModel>.Of(model.Id), model));
            }

            return result.AsReadOnly();
        }
        finally
        {
            _enumerationSemaphore.Release();
        }
    }

    public async ValueTask<IReadOnlyCollection<TId>> GetAllIdsAsync(IEntityModelStore<TId, TModel> store,
        CancellationToken token = default)
    {
        var result = await store.GetAllIdsAsync(token).ToListAsync(token);

        return result.AsReadOnly();
    }
}

internal interface IEntityBroker<TId, TEntity, in TActor, TModel> : IEntityBroker<TId, TEntity, TModel>
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
    ValueTask<TEntity?> GetImplicitAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IEntityModelStore<TId, TModel> store,
        TActor? actor = null,
        CancellationToken token = default);

    ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IEntityModelStore<TId, TModel> store,
        TActor? actor = null,
        CancellationToken token = default
    );

    ValueTask<TEntity?> IEntityBroker<TId, TEntity, TModel>.GetImplicitAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token
    ) => GetImplicitAsync(path, identity, store, null, token);

    ValueTask<IEntityHandle<TId, TEntity>?> IEntityBroker<TId, TEntity, TModel>.GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token
    ) => GetAsync(path, identity, store, null, token);
}

internal interface IEntityBroker<TId, TEntity, TModel> : IEntityBroker<TId, TEntity>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    ValueTask AttachLatentEntityAsync(TId id, TEntity entity, IEntityModelStore<TId, TModel> store,
        CancellationToken token);

    bool TryCreateLatentHandle(
        TModel model,
        [MaybeNullWhen(true)] out TEntity entity,
        [MaybeNullWhen(false)] out IDisposable handle,
        CancellationToken token
    );

    bool TryGetInScope(TId id, [MaybeNullWhen(false)] out TEntity entity);

    Task DestroyReferenceAsync(TId id, CancellationToken token);

    ValueTask UpdateAsync(TModel model, IEntityModelStore<TId, TModel> store, CancellationToken token);
    ValueTask BatchUpdateAsync(IEnumerable<TModel> model, IEntityModelStore<TId, TModel> store, CancellationToken token);


    ValueTask<TEntity?> GetImplicitAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token = default);

    ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token = default
    );

    ValueTask<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> GetAllHandlesAsync(
        CachePathable path,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token = default);

    ValueTask<IReadOnlyCollection<TEntity>> GetAllImplicitAsync(
        CachePathable path,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token = default);

    ValueTask<IReadOnlyCollection<TId>> GetAllIdsAsync(IEntityModelStore<TId, TModel> store,
        CancellationToken token = default);
}

internal interface IEntityBroker<in TId, in TEntity> : IEntityBroker<TId>
    where TId : IEquatable<TId>
    where TEntity : class, ICacheableEntity<TId>
{
    ValueTask ReleaseHandleAsync(IEntityHandle<TId, TEntity> handle);
    Type IEntityBroker.EntityType => typeof(TEntity);
}

internal interface IEntityBroker<in TId> : IEntityBroker;

internal interface IEntityBroker
{
    Type EntityType { get; }
}
