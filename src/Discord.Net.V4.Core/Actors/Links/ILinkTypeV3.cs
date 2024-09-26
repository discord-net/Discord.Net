using Discord.Models;
using MorseCode.ITask;

namespace Discord;

[LinkSchematic]
public partial interface ILinkTypeV3<out TActor, TId, out TEntity, in TModel> : 
    ILink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IModel
{
    [LinkSchematic]
    public partial interface Indexable : ISpecifiedLinkType<TActor, TId, TEntity, TModel>
    {
        TActor this[TId id] => GetActor(id);
        TActor Specifically(TId id) => GetActor(id);
    }
    
    [LinkSchematic(Children = [nameof(Indexable)])]
    public partial interface Enumerable : ISpecifiedLinkType<TActor, TId, TEntity, TModel>
    {
        ITask<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default);
    }

    [LinkSchematic(Children = [nameof(Indexable), nameof(Enumerable)])]
    public partial interface Defined : ISpecifiedLinkType<TActor, TId, TEntity, TModel>
    {
        IReadOnlyCollection<TId> Ids { get; }
    }

    [LinkSchematic(Children = [nameof(Indexable)])]
    public partial interface Paged<out TPaged, in TParams> :
        ISpecifiedLinkType<TActor, TId, TEntity, TModel>
        where TParams : IPagingParams
    {
        IAsyncPaged<TPaged> PagedAsync(TParams? args = default, RequestOptions? options = null);
    }
    
    [LinkSchematic(Children = [nameof(Indexable)])]
    public partial interface Paged<in TParams> :
        ISpecifiedLinkType<TActor, TId, TEntity, TModel>
        where TParams : IPagingParams
    {
        IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null);
    }
}