namespace Discord.Rest.Actors;

public class RestLoadableRootActor<TActor, TId, TEntity>(
    Func<TId, TActor> actorFactory,
    Func<Task<IReadOnlyCollection<TEntity>>> allFactory
):
    RestRootActor<TActor, TId, TEntity>(actorFactory),
    ILoadableRootActor<TActor, TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    public Task<IReadOnlyCollection<TEntity>> AllAsync()
        => allFactory();
}
