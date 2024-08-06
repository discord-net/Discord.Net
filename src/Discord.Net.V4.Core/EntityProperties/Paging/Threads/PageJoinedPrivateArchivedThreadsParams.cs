using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public sealed record PageJoinedPrivateArchivedThreadsParams(
    int? PageSize = null,
    int? Total = null,
    DateTimeOffset? Before = null
) :
    BasePageThreadChannelsParams(PageSize, Total, Before),
    IPagingParams<PageJoinedPrivateArchivedThreadsParams, ChannelThreads>
{
    public static IApiOutRoute<ChannelThreads>? GetRoute(
        PageJoinedPrivateArchivedThreadsParams? self,
        IPathable path,
        ChannelThreads? lastRequest
    ) => GetRoute(self, GetRoute, path, lastRequest);

    private static IApiOutRoute<ChannelThreads> GetRoute(ulong parentId, DateTimeOffset? before, int pageSize)
        => Routes.ListJoinedPrivateArchivedThreads(parentId, before, pageSize);


}
