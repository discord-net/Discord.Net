using Discord.Models;
using Discord.Paging;
using MorseCode.ITask;

namespace Discord;

public interface ISpecifiedLinkType<out TActor, TId, out TEntity, in TModel> :
    ILink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    public interface BackLink<out TSource> : 
        ILink<TActor, TId, TEntity, TModel>,
        IBackLink<TSource, TActor, TId, TEntity, TModel> 
        where TSource : class, IPathable;
}

public partial interface ILinkType<out TActor, TId, out TEntity, in TModel> : 
    ILink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    public interface BackLink<out TSource> : 
        ILink<TActor, TId, TEntity, TModel>,
        IBackLink<TSource, TActor, TId, TEntity, TModel> 
        where TSource : class, IPathable;
    
    [BackLinkable]
    public partial interface Indexable : 
        ISpecifiedLinkType<TActor, TId, TEntity, TModel>
    {
        TActor this[TId id] => Specifically(id);

        TActor Specifically(TId id);
    }

    [BackLinkable]
    public partial interface Enumerable : 
        ISpecifiedLinkType<TActor, TId, TEntity, TModel>
    {
        ITask<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default);
        
        [BackLinkable]
        public partial interface Indexable :
            Enumerable,
            ILinkType<TActor, TId, TEntity, TModel>.Indexable;
    }
    
    [BackLinkable]
    public partial interface Paged<in TParams> : 
        ISpecifiedLinkType<TActor, TId, TEntity, TModel>
        where TParams : IPagingParams
    {
        IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null);

        [BackLinkable]
        public partial interface Indexable :
            Paged<TParams>,
            ILinkType<TActor, TId, TEntity, TModel>.Indexable;
    }
    
    [BackLinkable]
    public partial interface Paged<out TPaged, in TParams> : 
        ISpecifiedLinkType<TActor, TId, TEntity, TModel>
        where TParams : IPagingParams
    {
        IAsyncPaged<TPaged> PagedAsync(TParams? args = default, RequestOptions? options = null);

        [BackLinkable]
        public partial interface Indexable :
            Paged<TParams>,
            ILinkType<TActor, TId, TEntity, TModel>.Indexable;
    }

    [BackLinkable]
    public partial interface Defined : 
        ISpecifiedLinkType<TActor, TId, TEntity, TModel>
    {
        IReadOnlyCollection<TId> Ids { get; }
        
        [BackLinkable]
        public partial interface Indexable :
            Defined,
            ILinkType<TActor, TId, TEntity, TModel>.Indexable;
    }
}