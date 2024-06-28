namespace Discord;

public readonly record struct PageUserGuildsParams(
    int? PageSize = 200,
    int? Total = null,
    EntityOrId<ulong, IPartialGuild>? Before = null,
    EntityOrId<ulong, IPartialGuild>? After = null
) : IPagingParams;
