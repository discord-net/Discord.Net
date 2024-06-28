using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> PublicArchivedThreads(
        DiscordRestClient client,
        IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
        IIdentifiableEntityOrModel<ulong, RestGuildChannel> channel
    ) => ListThreads(client, guild, channel, Routes.ListPublicArchivedThreads);

    public static RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> PrivateArchivedThreads(
        DiscordRestClient client,
        IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
        IIdentifiableEntityOrModel<ulong, RestGuildChannel> channel
    ) => ListThreads(client, guild, channel, Routes.ListPrivateArchivedThreads);

    public static RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> JoinedPrivateArchivedThreads(
        DiscordRestClient client,
        IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
        IIdentifiableEntityOrModel<ulong, RestGuildChannel> channel
    ) => ListThreads(client, guild, channel, Routes.ListJoinedPrivateArchivedThreads);

    private static RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> ListThreads(
        DiscordRestClient client,
        IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
        IIdentifiableEntityOrModel<ulong, RestGuildChannel> channel,
        Func<ulong, DateTimeOffset?, int?, IApiOutRoute<ChannelThreads>> route)
    {
        return new RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams>(
            client,
            args => route(channel.Id, args.Before, args.PageSize),
            (_, model) => model.Threads
                .OfType<IThreadChannelModel>()
                .Select(x =>
                {
                    return RestThreadChannel.Construct(client, x, new(
                        guild,
                        model.Members.FirstOrDefault(member => member.Id.IsSpecified && member.Id.Value == x.Id)
                    ));
                }),
            (_, model, args) =>
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
                    channel.Id,
                    timestamp,
                    args.PageSize
                );
            }
        );
    }
}
