using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public sealed record PagePublicArchivedThreadsParams(
    int? PageSize = null,
    int? Total = null,
    DateTimeOffset? Before = null
) :
    BasePageThreadChannelsParams(PageSize, Total, Before),
    IPagingParams<PagePublicArchivedThreadsParams, ChannelThreads>
{
    public static IApiOutRoute<ChannelThreads>? GetRoute(
        PagePublicArchivedThreadsParams? self,
        IPathable path,
        ChannelThreads? lastRequest
    ) => GetRoute(self, GetRoute, path, lastRequest);

    private static IApiOutRoute<ChannelThreads> GetRoute(ulong parentId, DateTimeOffset? before, int pageSize)
        => Routes.ListPublicArchivedThreads(parentId, before, pageSize);
}
