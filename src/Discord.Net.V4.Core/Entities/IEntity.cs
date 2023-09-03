namespace Discord;

public interface IEntity<T>
    where T : IEquatable<T>
{
    T Id { get; }
}
