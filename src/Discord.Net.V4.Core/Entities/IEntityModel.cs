namespace Discord;

public interface IEntityModel<TId>
    where TId : IEquatable<TId>
{
    TId Id { get; }
}
