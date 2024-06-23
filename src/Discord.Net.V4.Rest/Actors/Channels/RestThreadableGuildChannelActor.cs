using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest.Channels;

namespace Discord.Rest;

public partial class RestLoadableThreadableGuildChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestThreadableGuildChannelActor(client, guildId, id),
    ILoadableThreadableGuildChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildChannel>))]
    internal RestLoadable<ulong, RestGuildChannel, IGuildChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestGuildChannel, GuildChannelBase>(
                (_, model) => RestGuildChannel.Construct(client, model, guildId)
            )
        );
}

public partial class RestThreadableGuildChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestGuildChannelActor(client, guildId, id),
    IThreadableGuildChannelActor
{
    public RestPagedActor<ulong, RestThreadChannel, ChannelThreads> PublicArchivedThreads { get; }
        = RestActors.PublicArchivedThreads(client, guildId, id);

    public RestPagedActor<ulong, RestThreadChannel, ChannelThreads> PrivateArchivedThreads { get; }
        = RestActors.PrivateArchivedThreads(client, guildId, id);

    public RestPagedActor<ulong, RestThreadChannel, ChannelThreads> JoinedPrivateArchivedThreads { get; }
        = RestActors.JoinedPrivateArchivedThreads(client, guildId, id);

    IPagedActor<ulong, IThreadChannel> IThreadableGuildChannelActor.PublicArchivedThreads => PublicArchivedThreads;
    IPagedActor<ulong, IThreadChannel> IThreadableGuildChannelActor.PrivateArchivedThreads => PrivateArchivedThreads;
    IPagedActor<ulong, IThreadChannel> IThreadableGuildChannelActor.JoinedPrivateArchivedThreads => JoinedPrivateArchivedThreads;
}
