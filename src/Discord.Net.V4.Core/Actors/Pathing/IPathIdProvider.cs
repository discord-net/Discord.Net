namespace Discord;

public interface IPathIdProvider<out T> : IIdentifiable<T>
    where T : IEquatable<T>;