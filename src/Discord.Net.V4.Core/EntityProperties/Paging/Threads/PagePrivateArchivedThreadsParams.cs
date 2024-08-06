using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public sealed record PagePrivateArchivedThreadsParams(
    int? PageSize = null,
    int? Total = null,
    DateTimeOffset? Before = null
) :
    BasePageThreadChannelsParams(PageSize, Total, Before),
    IPagingParams<PagePrivateArchivedThreadsParams, ChannelThreads>
{
    public static IApiOutRoute<ChannelThreads>? GetRoute(
        PagePrivateArchivedThreadsParams? self,
        IPathable path,
        ChannelThreads? lastRequest
    ) => GetRoute(self, GetRoute, path, lastRequest);

    private static IApiOutRoute<ChannelThreads> GetRoute(ulong parentId, DateTimeOffset? before, int pageSize)
        => Routes.ListPrivateArchivedThreads(parentId, before, pageSize);
}
