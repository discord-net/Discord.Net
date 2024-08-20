using Discord.Models;
using Discord.Paging;

// ReSharper disable IdentifierTypo

namespace Discord;

public interface ILink<out TActor, TId, out TEntity, in TModel> :
    IEntityProvider<TEntity, TModel>,
    IPathable
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>;

[BackLinkable]
public partial interface ILinkType<out TActor, TId, TEntity, in TModel> : 
    ILink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    [BackLinkable]
    public partial interface Indexable : 
        ILinkType<TActor, TId, TEntity, TModel>
    {
        TActor this[TId id] => Specifically(id);

        TActor Specifically(TId id);
    }

    [BackLinkable]
    public partial interface Enumerable : 
        ILinkType<TActor, TId, TEntity, TModel>
    {
        ValueTask<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default);
        
        [BackLinkable]
        public partial interface Indexable :
            Enumerable,
            ILinkType<TActor, TId, TEntity, TModel>.Indexable;
    }
    
    [BackLinkable]
    public partial interface Paged<in TParams> : 
        ILinkType<TActor, TId, TEntity, TModel>
        where TParams : IPagingParams
    {
        IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null);

        [BackLinkable]
        public partial interface Indexable :
            Paged<TParams>,
            ILinkType<TActor, TId, TEntity, TModel>.Indexable;
    }

    [BackLinkable]
    public partial interface Defined : 
        ILinkType<TActor, TId, TEntity, TModel>
    {
        IReadOnlyCollection<TId> Ids { get; }
        
        [BackLinkable]
        public partial interface Indexable :
            Defined,
            ILinkType<TActor, TId, TEntity, TModel>.Indexable;
    }
}

public interface IBackLink<out TSource, out TActor, TId, out TEntity, in TModel>
    where TSource : class, IPathable
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    internal TSource Source { get; }
}

public interface IBackLinkedActor<out TSource, out TBackLink, out TActor, TId, TEntity, in TModel> : 
    ILinkType<TBackLink, TId, TEntity, TModel>,
    IBackLink<TSource, TActor, TId, TEntity, TModel>
    where TBackLink : class, IBackLink<TSource, TActor, TId, TEntity, TModel>, TActor
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
    where TSource : class, IPathable;