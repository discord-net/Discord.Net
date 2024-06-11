namespace Discord;

public interface IEntity<out T> : IEntity, IIdentifiable<T> where T : IEquatable<T>;

public interface IEntity : IClientProvider;
