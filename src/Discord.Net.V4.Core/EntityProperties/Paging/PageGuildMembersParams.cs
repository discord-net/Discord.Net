namespace Discord;

public readonly record struct PageGuildMembersParams(
    int? PageSize = 1000,
    int? Total = null,
    EntityOrId<ulong, IGuildMember>? After = null
) : IPagingParams;
