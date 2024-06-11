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
}
