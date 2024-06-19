using Discord.Paging;

namespace Discord.Rest.Actors;

public class RestPagedLoadableRootActor<TActor, TId, TEntity>(
    Func<TId, TActor> actorFactory,
    Func<IAsyncPaged<TEntity>> pageFactory
) :
    RestPagedLoadableRootActor<TActor, TId, TEntity, TEntity>(actorFactory, pageFactory),
    IPagedLoadableRootActor<TActor, TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>;

public class RestPagedLoadableRootActor<TActor, TId, TEntity, TPagedEntity>(
    Func<TId, TActor> actorFactory,
    Func<IAsyncPaged<TPagedEntity>> pageFactory
):
    RestRootActor<TActor, TId, TEntity>(actorFactory),
    IPagedLoadableRootActor<TActor, TId, TEntity, TPagedEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TPagedEntity : class, IEntity<TId>
{
    public IAsyncPaged<TPagedEntity> PagedAsync()
        => pageFactory();
}
