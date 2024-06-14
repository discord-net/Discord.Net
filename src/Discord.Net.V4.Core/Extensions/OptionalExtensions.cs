using System.Runtime.CompilerServices;

namespace Discord;

public static class OptionalExtensions
{
    public static Optional<U> ToApiModel<T, U>(this Optional<T> optional)
        where T : IEntityProperties<U>
    {
        return optional.Map(v => v.ToApiModel());
    }

    public static Optional<T> OptionalIf<T>(this T value, Predicate<T> predicate)
        => predicate(value) ? Optional.Some(value) : Optional<T>.Unspecified;

    public static Optional<int> MapToInt<T>(this Optional<T> optional)
        where T : unmanaged, Enum
        => optional.Map(v => Unsafe.As<T, int>(ref v));

    public static Optional<T> UnspecifiedIfNull<T>(Optional<T?> optional)
        where T : struct
        => optional.Map(v => v is null ? Optional<T>.Unspecified : Optional.Some(v.Value));

    public static Optional<int?> MapToInt<T>(this Optional<T?> optional)
        where T : unmanaged, Enum
        => optional.Map(v => v?.GetHashCode());

    public static Optional<TId> MapToId<TId, TEntity>(this Optional<EntityOrId<TId, TEntity>> optional)
        where TId : IEquatable<TId>
        where TEntity : IIdentifiable<TId>
        => optional.Map(v => v.Id);

    public static Optional<TId?> MapToId<TId, TEntity>(this Optional<EntityOrId<TId, TEntity>?> optional)
        where TId : class, IEquatable<TId>
        where TEntity : IIdentifiable<TId>
        => optional.Map(v => v?.Id);

    public static Optional<TId?> MapToNullableId<TId, TEntity>(this Optional<EntityOrId<TId, TEntity>?> optional)
        where TId : struct, IEquatable<TId>
        where TEntity : IIdentifiable<TId>
        => optional.Map(v => v?.Id);
}
