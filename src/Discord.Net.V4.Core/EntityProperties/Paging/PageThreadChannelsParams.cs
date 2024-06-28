namespace Discord;

public readonly record struct PageThreadChannelsParams(
    int? PageSize = null,
    int? Total = null,
    DateTimeOffset? Before = null
) : IPagingParams;
