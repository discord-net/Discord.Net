using Discord.Models;
using Discord.Rest;

namespace Discord;

public sealed record PageGuildMembersParams(
    int? PageSize = DiscordConfig.MaxUsersPerBatch,
    int? Total = null,
    EntityOrId<ulong, IMember>? After = null
) : IDirectionalPagingParams<ulong>, IPagingParams<PageGuildMembersParams, IEnumerable<IMemberModel>>
{
    public static int MaxPageSize => DiscordConfig.MaxUsersPerBatch;

    public Direction? Direction => After.Map(Discord.Direction.After);

    public Optional<ulong> From => Optional.FromNullable(After?.Id);

    public static IApiOutRoute<IEnumerable<IMemberModel>>? GetRoute(
        PageGuildMembersParams? self,
        IPathable path,
        IEnumerable<IMemberModel>? lastRequest)
    {
        if (!path.TryGet<ulong, IGuild>(out var guildId))
            return null;

        var pageSize = IPagingParams.GetPageSize(self);

        if (lastRequest is null)
        {
            return Routes.ListGuildMembers(
                guildId,
                pageSize,
                self?.After
            );
        }
        return Routes.ListGuildMembers(
            guildId,
            pageSize,
            lastRequest.MaxBy(x => x.Id)?.Id
        );
    }
}
