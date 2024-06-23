using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Channels;

public partial class RestLoadableNewsChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestNewsChannelActor(client, guildId, id),
    ILoadableNewsChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<INewsChannel>))]
    internal RestLoadable<ulong, RestNewsChannel, INewsChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestNewsChannel, GuildAnnouncementChannel>(
                (_, model) => RestNewsChannel.Construct(client, model, guildId)
            )
        );
}

[ExtendInterfaceDefaults(typeof(INewsChannelActor))]
public partial class RestNewsChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestGuildChannelActor(client, guildId, id),
    INewsChannelActor
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, guildId, id);

    [ProxyInterface(typeof(IThreadableGuildChannelActor))]
    internal RestThreadableGuildChannelActor ThreadableActor { get; } = new(client, guildId, id);
}

public partial class RestNewsChannel(DiscordRestClient client, ulong guildId, IGuildNewsChannelModel model) :
    RestTextChannel(client, guildId, model),
    INewsChannel,
    IContextConstructable<RestNewsChannel, IGuildNewsChannelModel, ulong, DiscordRestClient>
{
    public static RestNewsChannel Construct(DiscordRestClient client, IGuildNewsChannelModel model, ulong context)
        => new(client, context, model);
}
