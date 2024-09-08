using Discord.Models;
using Discord.Rest;

namespace Discord;

public sealed record PageThreadMembersParams(
    int? PageSize = null,
    int? Total = null,
    EntityOrId<ulong, IUserActor>? After = null
) : IDirectionalPagingParams<ulong>, IPagingParams<PageThreadMembersParams, IEnumerable<IThreadMemberModel>>
{
    public static int MaxPageSize => DiscordConfig.MaxThreadMembersPerBatch;

    public Direction? Direction => After.Map(Discord.Direction.After);

    public Optional<ulong> From => Optional.FromNullable(After?.Id);

    public static IApiOutRoute<IEnumerable<IThreadMemberModel>>? GetRoute(
        PageThreadMembersParams? self, 
        IPathable path,
        IEnumerable<IThreadMemberModel>? lastRequest)
    {
        if (!path.TryGet<ulong, IThreadChannel>(out var threadId))
            return null;

        var pageSize = IPagingParams.GetPageSize(self);

        if (lastRequest is null)
        {
            return Routes.ListThreadMembersPaged(
                threadId,
                self?.After,
                pageSize
            );
        }

        return Routes.ListThreadMembersPaged(
            threadId,
            lastRequest.MaxBy(x => x.Id)?.Id,
            pageSize
        );
    }
}