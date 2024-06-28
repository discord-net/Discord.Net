namespace Discord;

public readonly record struct PageGuildBansParams(
    int? PageSize = null,
    int? Total = null,
    EntityOrId<ulong, IUser>? Before = null,
    EntityOrId<ulong, IUser>? After = null
) : IPagingParams;
