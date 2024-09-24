namespace Discord;

public interface IPathIdProvider<out TId>
    where TId : IEquatable<TId>
{
    TId Id { get; }
}