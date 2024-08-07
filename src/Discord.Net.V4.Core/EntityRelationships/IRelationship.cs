namespace Discord;

public interface IRelationship<out TActor, out TId, out TEntity> :
    IRelation<TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>
{
    internal TActor RelationshipActor { get; }

    TId IRelation<TId, TEntity> .RelationshipId => RelationshipActor.Id;
}

public interface IRelation<out TId, out TEntity>
    where TId : IEquatable<TId>
{
    TId RelationshipId { get; }
}
