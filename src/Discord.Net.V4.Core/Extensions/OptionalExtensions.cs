using System.Runtime.CompilerServices;

namespace Discord;

public static class OptionalExtensions
{
    public static Optional<int> MapToInt<T>(this Optional<T> optional)
        where T : unmanaged, Enum
        => optional.Map(v => Unsafe.As<T, int>(ref v));

    public static Optional<int?> MapToInt<T>(this Optional<T?> optional)
        where T : unmanaged, Enum
        => optional.Map(v => v?.GetHashCode());

    public static Optional<TId?> MapToNullableId<TId, TEntity>(this Optional<IdOrEntity<TId, TEntity>?> optional)
        where TId : struct, IEquatable<TId>
        where TEntity : IIdentifiable<TId>
        => optional.Map(v => v?.Id);
}