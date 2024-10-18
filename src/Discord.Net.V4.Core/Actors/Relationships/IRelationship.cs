namespace Discord;

public interface IRelationship<out TActor, out TId, out TEntity> :
    IRelation<TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>
{
    internal TActor RelationshipActor { get; }

    TId IRelation<TId, TEntity>.RelationshipId => RelationshipActor.Id;
}