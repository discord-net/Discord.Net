namespace Discord;

public interface ISnowflakeIdentifiable : IIdentifiable<ulong>;

public interface IIdentifiable<out TId> where TId : IEquatable<TId>
{
    TId Id { get; }
}
