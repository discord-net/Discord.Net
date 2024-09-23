namespace Discord;

public interface IPathIdProvider<out T>
    where T : IEquatable<T>;