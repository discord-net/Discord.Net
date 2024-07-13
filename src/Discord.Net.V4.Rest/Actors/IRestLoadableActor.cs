using Discord.Models;

namespace Discord.Rest;

public interface IRestLoadableActor<TId, TEntity, TCore, TModel>
    where TCore : class, IEntity<TId>
    where TEntity : class, IEntity<TId>, IEntityOf<TModel>, TCore
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    internal RestLoadable<TId, TEntity, TCore, TModel> Loadable { get; }
}
