using Discord.Models;

namespace Discord.Rest;

public interface IRestLinkType<out TActor, in TId, out TEntity, in TModel> :
    IRestClientProvider,
    ILink<TActor, TId, TEntity, TModel>
    where TActor : class, IRestActor<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TModel : class, IModel
{
    internal IActorProvider<TActor, TId> Provider { get; }

    TActor IActorProvider<TActor, TId>.GetActor(TId id)
        => Provider.GetActor(id);
}