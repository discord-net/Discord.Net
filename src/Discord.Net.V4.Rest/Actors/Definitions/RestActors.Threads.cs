using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> PublicArchivedThreads(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadableChannelIdentity channel
    ) => ListThreads(client, guild, channel, Routes.ListPublicArchivedThreads);

    public static RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> PrivateArchivedThreads(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadableChannelIdentity channel
    ) => ListThreads(client, guild, channel, Routes.ListPrivateArchivedThreads);

    public static RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> JoinedPrivateArchivedThreads(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadableChannelIdentity channel
    ) => ListThreads(client, guild, channel, Routes.ListJoinedPrivateArchivedThreads);

    private static RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> ListThreads(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadableChannelIdentity channel,
        Func<ulong, DateTimeOffset?, int?, IApiOutRoute<ChannelThreads>> route)
    {
        return new RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams>(
            client,
            args => route(channel.Id, args.Before, args.PageSize),
            (_, model) => model.Threads
                .OfType<IThreadChannelModel>()
                .Select(x => RestThreadChannel.Construct(client, x, new(guild))),
            (_, model, args) =>
            {
                if (!model.HasMore)
                    return null;

                var timestamp = model.Threads
                    .OfType<ThreadChannelModelBase>()
                    .MinBy(x => x.Metadata.ArchiveTimestamp)?
                    .Metadata
                    .ArchiveTimestamp;

                if (timestamp is null)
                    return null;

                return route(
                    channel.Id,
                    timestamp,
                    args.PageSize
                );
            }
        );
    }
}
