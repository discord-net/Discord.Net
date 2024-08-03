namespace Discord;

public record PageGuildBansParams(
    int? PageSize = null,
    int? Total = null,
    EntityOrId<ulong, IUser>? Before = null,
    EntityOrId<ulong, IUser>? After = null
) : IBetweenPagingParams<ulong>
{
    public static int MaxPageSize => DiscordConfig.MaxBansPerBatch;

    Optional<ulong> IBetweenPagingParams<ulong>.Before => Optional.FromNullable(Before?.Id);

    Optional<ulong> IBetweenPagingParams<ulong>.After => Optional.FromNullable(After?.Id);
}
