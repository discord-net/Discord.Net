namespace Discord;

public interface IEntity<T> : IEntity
    where T : IEquatable<T>
{
    T Id { get; }
}

public interface IEntity
{
    IDiscordClient Client { get; }
}
