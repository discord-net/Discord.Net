namespace Discord;

public interface IEntity<out T> : IEntity
    where T : IEquatable<T>
{
    T Id { get; }
}

public interface IEntity : IClientProvider
{

}
