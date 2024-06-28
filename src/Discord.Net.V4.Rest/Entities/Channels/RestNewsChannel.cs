using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;

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
public partial class RestNewsChannelActor(DiscordRestClient client, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild, ulong id) :
    RestGuildChannelActor(client, guild, id),
    INewsChannelActor
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, guild, id);

    [ProxyInterface(typeof(IThreadableGuildChannelActor))]
    internal RestThreadableGuildChannelActor ThreadableActor { get; } = new(client, guild, id);
}

public partial class RestNewsChannel(DiscordRestClient client, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild, IGuildNewsChannelModel model) :
    RestTextChannel(client, guild, model),
    INewsChannel,
    IContextConstructable<RestNewsChannel, IGuildNewsChannelModel, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel>, DiscordRestClient>
{
    public static RestNewsChannel Construct(DiscordRestClient client, IGuildNewsChannelModel model, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild)
        => new(client, guild, model);
}
