using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Discord.Models;

namespace Discord.Rest;

public abstract class RestActor<TSelf, TId, TEntity, TModel> :
    IRestActor<TId, TEntity, IIdentifiable<TId, TEntity, TSelf, TModel>, TModel>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>, IRestConstructable<TEntity, TSelf, TModel>
    where TSelf : RestActor<TSelf, TId, TEntity, TModel>
    where TModel : class, IEntityModel<TId>
{
    public DiscordRestClient Client { get; }

    public TId Id { get; }

    internal abstract IIdentifiable<TId, TEntity, TSelf, TModel> Identity { get; }

    private WeakReference<TEntity>? _entityReference;
    private readonly WeakQueue<TModel> _modelQueue;
    
    protected RestActor(DiscordRestClient client,
        IIdentifiable<TId, TEntity, TSelf, TModel> identity)
    {
        _modelQueue = new();
        
        Client = client;
        Id = identity.Id;

        if (identity.Detail <= IdentityDetail.Actor) return;
        
        var entity = identity.Entity;
        if(entity is not null) _entityReference = new(entity);
    }

    internal bool TryGetPreCachedEntity([MaybeNullWhen(false)] out TEntity entity)
    {
        if (TryGetBoundEntity(out entity))
            return true;

        if (_modelQueue.TryDequeue(out var model))
        {
            lock (Identity)
            {
                _modelQueue.Clear();
            
                entity = CreateEntity(model);
                BindUnlocked(entity);
            }
            
            return true;
        }

        entity = null;
        return false;
    }
    
    protected void Unbind()
    {
        lock(Identity)
            _entityReference = null;
    }
    
    protected void Bind(TEntity entity)
    {
        lock (Identity) BindUnlocked(entity);
    }

    private void BindUnlocked(TEntity entity)
    {
        if (_entityReference is null)
            _entityReference = new(entity);
        else
            _entityReference.SetTarget(entity);
    }

    protected bool TryGetBoundEntity([MaybeNullWhen(false)] out TEntity entity)
    {
        lock (Identity)
        {
            if (_entityReference is not null) 
                return _entityReference.TryGetTarget(out entity);
        }
        
        entity = null;
        return false;
    }

    internal void AddModelSource(TModel model)
    {
        if (TryGetBoundEntity(out var entity) && entity is IUpdatable<TModel> updatable)
        {
            Task.Run(async () => await updatable.UpdateAsync(model));
            return;
        }
        
        lock(Identity) _modelQueue.Enqueue(model);
    }
    
    internal virtual TEntity CreateEntity(TModel model)
        => TEntity.Construct(Client, (TSelf)this, model);

    IIdentifiable<TId, TEntity, TSelf, TModel>
        IRestActor<TId, TEntity, IIdentifiable<TId, TEntity, TSelf, TModel>, TModel>
        .Identity => Identity;

    TEntity IEntityProvider<TEntity, TModel>.CreateEntity(TModel model) => CreateEntity(model);
}

public interface IRestActor<out TId, out TEntity, out TIdentity, in TModel> :
    IRestActor<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId, TModel>
    where TModel : IEntityModel<TId>
{
    internal TIdentity Identity { get; }
}

public interface IRestActor<out TId, out TEntity, in TModel> :
    IRestActor<TId, TEntity>,
    IEntityProvider<TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId, TModel>
    where TModel : IEntityModel<TId>;

public interface IRestActor<out TId, out TEntity> :
    IActor<TId, TEntity>,
    IRestClientProvider
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;