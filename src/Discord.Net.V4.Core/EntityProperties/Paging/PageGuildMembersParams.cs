namespace Discord;

public readonly record struct PageGuildMembersParams(
    int? PageSize = DiscordConfig.MaxUsersPerBatch,
    int? Total = null,
    EntityOrId<ulong, IGuildMember>? After = null
) : IDirectionalPagingParams<ulong>
{
    public static int MaxPageSize => DiscordConfig.MaxUsersPerBatch;

    public Direction? Direction => After.Map(Discord.Direction.After);

    public Optional<ulong> From => Optional.FromNullable(After?.Id);
}
