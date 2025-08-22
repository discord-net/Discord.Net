namespace Discord;

public interface IIdentifiable<out TId>
    where TId : IEquatable<TId>
{
    TId Id { get; }
}