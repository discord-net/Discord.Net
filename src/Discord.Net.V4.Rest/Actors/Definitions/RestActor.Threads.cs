using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static RestPagedActor<ulong, RestThreadChannel, ChannelThreads> PublicArchivedThreads(
        DiscordRestClient client, ulong guildId, ulong channelId)
        => ListThreads(client, guildId, channelId, Routes.ListPublicArchivedThreads);

    public static RestPagedActor<ulong, RestThreadChannel, ChannelThreads> PrivateArchivedThreads(
        DiscordRestClient client, ulong guildId, ulong channelId)
        => ListThreads(client, guildId, channelId, Routes.ListPrivateArchivedThreads);

    public static RestPagedActor<ulong, RestThreadChannel, ChannelThreads> JoinedPrivateArchivedThreads(
        DiscordRestClient client, ulong guildId, ulong channelId)
        => ListThreads(client, guildId, channelId, Routes.ListJoinedPrivateArchivedThreads);

    private static RestPagedActor<ulong, RestThreadChannel, ChannelThreads> ListThreads(
        DiscordRestClient client,
        ulong guildId,
        ulong channelId,
        Func<ulong, DateTimeOffset?, int?, IApiOutRoute<ChannelThreads>> route)
    {
        return new RestPagedActor<ulong, RestThreadChannel, ChannelThreads>(
            client,
            route(channelId, null, null),
            (_, model) => model.Threads
                .OfType<IThreadChannelModel>()
                .Select(x =>
                {
                    return RestThreadChannel.Construct(client, x, new(
                        guildId,
                        model.Members.FirstOrDefault(member => member.Id.IsSpecified && member.Id.Value == x.Id)
                    ));
                }),
            (pager, model) =>
            {
                if (!model.HasMore)
                    return null;

                var timestamp = model.Threads
                    .OfType<ThreadChannelBase>()
                    .MinBy(x => x.Metadata.ArchiveTimestamp)?
                    .Metadata
                    .ArchiveTimestamp;

                if (timestamp is null)
                    return null;

                return route(
                    channelId,
                    timestamp,
                    pager.PageSize
                );
            }
        );
    }
}
