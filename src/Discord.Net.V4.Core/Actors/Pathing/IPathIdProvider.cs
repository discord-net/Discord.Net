namespace Discord;

public interface IPathIdProvider<out TId>
    where TId : IEquatable<TId>
{
    internal TId PathId { get; }
}