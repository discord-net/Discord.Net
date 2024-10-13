namespace Discord;

public interface IRelation<out TId, out TEntity>
    where TId : IEquatable<TId>
{
    internal TId RelationshipId { get; }
}