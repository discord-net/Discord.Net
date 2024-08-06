using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public abstract record BasePageThreadChannelsParams(
    int? PageSize = null,
    int? Total = null,
    DateTimeOffset? Before = null
) : IDirectionalPagingParams<DateTimeOffset>
{
    public static int MaxPageSize => DiscordConfig.MaxThreadsPerBatch;

    public Direction? Direction => Before.Map(Discord.Direction.Before);

    public Optional<DateTimeOffset> From => Optional.FromNullable(Before);

    protected static IApiOutRoute<ChannelThreads>? GetRoute(
        BasePageThreadChannelsParams? self,
        GetRouteDelegate getRoute,
        IPathable path,
        ChannelThreads? lastRequest)
    {
        if (!path.TryGet<ulong, IThreadableChannel>(out var parentId))
            return null;

        var pageSize = IPagingParams.GetPageSize(self);

        if (lastRequest is null)
            return getRoute(parentId, self?.Before, pageSize);

        if (!lastRequest.HasMore || lastRequest.Threads.Length != pageSize) return null;

        var min = lastRequest.Threads
            .MinBy(x => x.Metadata.CreatedAt | SnowflakeUtils.FromSnowflake(x.Id));

        if (min is null)
            return null;

        var minDate = min.Metadata.CreatedAt | SnowflakeUtils.FromSnowflake(min.Id);

        return getRoute(parentId, minDate, pageSize);
    }

    protected delegate IApiOutRoute<ChannelThreads> GetRouteDelegate(ulong parentId, DateTimeOffset? before, int pageSize);
}
