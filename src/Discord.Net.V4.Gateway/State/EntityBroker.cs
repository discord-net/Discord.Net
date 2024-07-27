using Discord.Gateway.Cache;
using Microsoft.Extensions.Logging;

namespace Discord.Gateway.State;

internal sealed class EntityBroker<TId, TEntity, TModel> : IEntityBroker<TId, TEntity, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IContextConstructable<TEntity, TModel, IPathable, DiscordGatewayClient>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    private sealed class EntityReference(
        EntityBroker<TId, TEntity, TModel> broker,
        TId id,
        TEntity entity,
        params IEntityHandle<TId, TEntity>[] handles)
    {
        public bool IsAlive => Handles.Count > 0 || WeakReference.TryGetTarget(out _);

        public TId Id { get; } = id;

        public WeakReference<TEntity> WeakReference { get; } = new(entity);

        public List<IEntityHandle<TId, TEntity>> Handles { get; } = handles.ToList();

        public IEntityHandle<TId, TEntity>? AllocateHandle()
        {
            if (!WeakReference.TryGetTarget(out var entity))
                return null;

            var handle = new EntityHandle<TId, TEntity>(broker, Id, entity);
            Handles.Add(handle);
            return handle;
        }
    }

    private readonly DiscordGatewayClient _client;
    private readonly StateController _controller;
    private readonly Dictionary<TId, EntityReference> _references;
    private readonly KeyedSemaphoreSlim<IIdentifiable<TId, TEntity, TModel>> _keyedSemaphore;
    private readonly SemaphoreSlim _enumerationSemaphore = new(1, 1);

    private readonly ILogger<EntityBroker<TId, TEntity, TModel>> _logger;

    public EntityBroker(
        DiscordGatewayClient client,
        StateController stateController)
    {
        _keyedSemaphore = new(1, 1);
        _client = client;
        _controller = stateController;
        _logger = client.LoggerFactory.CreateLogger<EntityBroker<TId, TEntity, TModel>>();
        _references = new();
    }

    private void RemoveDeadReference(EntityReference reference)
    {
        if (reference.IsAlive) return;

        _references.Remove(reference.Id);
    }

    private EntityHandle<TId, TEntity> CreateReferenceAndHandle(
        IPathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        TModel model)
    {
        var entity = TEntity.Construct(_client, path, model);

        var handle = new EntityHandle<TId, TEntity>(this, identity.Id, entity);

        _references.Add(identity.Id, new EntityReference(this, identity.Id, entity, handle));

        return handle;
    }

    public async ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        IPathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token = default)
    {
        using var scope = _keyedSemaphore.Get(identity, out var semaphoreSlim);

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

            return CreateReferenceAndHandle(path, identity, model);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }


    public async ValueTask<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> GetAllAsync(
        IPathable path,
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

    public async ValueTask<IReadOnlyCollection<TId>> GetAllIdsAsync(IEntityModelStore<TId, TModel> store, CancellationToken token = default)
    {
        var result = await store.GetAllIdsAsync(token).ToListAsync(token);

        return result.AsReadOnly();
    }
}

internal interface IEntityBroker<TId, TEntity, TModel> : IEntityBroker<TId, TEntity>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IContextConstructable<TEntity, TModel, IPathable, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        IPathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token = default
    );

    ValueTask<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> GetAllAsync(
        IPathable path,
        IEntityModelStore<TId, TModel> store,
        CancellationToken token = default);

    ValueTask<IReadOnlyCollection<TId>> GetAllIdsAsync(IEntityModelStore<TId, TModel> store, CancellationToken token = default);
}

internal interface IEntityBroker<in TId, TEntity> : IEntityBroker<TId>
    where TId : IEquatable<TId>
    where TEntity : class, ICacheableEntity<TId>
{
    Type IEntityBroker.EntityType => typeof(TEntity);
}

internal interface IEntityBroker<in TId> : IEntityBroker;

internal interface IEntityBroker
{
    Type EntityType { get; }
}
