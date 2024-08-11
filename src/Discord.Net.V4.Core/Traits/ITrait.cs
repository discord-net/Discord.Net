namespace Discord;

public interface IActorTrait<out TId, out TEntity> :
    IActor<TId, TEntity>,
    ITrait<TId>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;

public interface ITrait<out TId> :
    IIdentifiable<TId>,
    IPathable
    where TId : IEquatable<TId>;
