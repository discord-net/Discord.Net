namespace Discord;

public readonly record struct PageThreadChannelsParams(
    int? PageSize = null,
    int? Total = null,
    DateTimeOffset? Before = null
) : IDirectionalPagingParams<DateTimeOffset>
{
    public static int MaxPageSize => DiscordConfig.MaxThreadsPerBatch;

    public Direction? Direction => Before.Map(Discord.Direction.Before);

    public Optional<DateTimeOffset> From => Optional.FromNullable(Before);
}
