namespace Discord;

public sealed record PageUserGuildsParams(
    int? PageSize = 200,
    int? Total = null,
    EntityOrId<ulong, IPartialGuild>? Before = null,
    EntityOrId<ulong, IPartialGuild>? After = null
) : IBetweenPagingParams<ulong>
{
    public static int MaxPageSize => DiscordConfig.MaxUsersGuildsPerBatch;

    Optional<ulong> IBetweenPagingParams<ulong>.Before => Optional.FromNullable(Before?.Id);

    Optional<ulong> IBetweenPagingParams<ulong>.After => Optional.FromNullable(After?.Id);
}
