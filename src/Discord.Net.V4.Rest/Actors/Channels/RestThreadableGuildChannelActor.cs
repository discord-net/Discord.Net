using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;

namespace Discord.Rest;

public partial class RestLoadableThreadableGuildChannelActor(
    DiscordRestClient client,
    IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
    IIdentifiableEntityOrModel<ulong, RestGuildChannel, IGuildChannelModel> channel
):
    RestThreadableGuildChannelActor(client, guild, channel),
    ILoadableThreadableGuildChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildChannel>))]
    internal RestLoadable<ulong, RestGuildChannel, IGuildChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestGuildChannel, GuildChannelBase>(
                (_, model) => RestGuildChannel.Construct(client, model, guild)
            )
        );
}

public partial class RestThreadableGuildChannelActor(
    DiscordRestClient client,
    IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
    IIdentifiableEntityOrModel<ulong, RestGuildChannel, IGuildChannelModel> channel
):
    RestGuildChannelActor(client, guild, channel),
    IThreadableGuildChannelActor
{
    public RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> PublicArchivedThreads { get; }
        = RestActors.PublicArchivedThreads(client, guild, channel);

    public RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> PrivateArchivedThreads { get; }
        = RestActors.PrivateArchivedThreads(client, guild, channel);

    public RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams> JoinedPrivateArchivedThreads { get; }
        = RestActors.JoinedPrivateArchivedThreads(client, guild, channel);

    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> IThreadableGuildChannelActor.PublicArchivedThreads => PublicArchivedThreads;
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> IThreadableGuildChannelActor.PrivateArchivedThreads => PrivateArchivedThreads;
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> IThreadableGuildChannelActor.JoinedPrivateArchivedThreads => JoinedPrivateArchivedThreads;
}
