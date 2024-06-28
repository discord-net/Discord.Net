using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;

namespace Discord.Rest.Channels;

using ChannelIdentity = IdentifiableEntityOrModel<ulong, RestNewsChannel, IGuildNewsChannelModel>;

public partial class RestLoadableNewsChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ChannelIdentity channel
):
    RestNewsChannelActor(client, guild, channel),
    ILoadableNewsChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<INewsChannel>))]
    internal RestLoadable<ulong, RestNewsChannel, INewsChannel, Channel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestNewsChannel, GuildAnnouncementChannel>(
                (_, model) => RestNewsChannel.Construct(client, model, guild)
            )
        );
}

[ExtendInterfaceDefaults(typeof(INewsChannelActor))]
public partial class RestNewsChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ChannelIdentity channel
):
    RestTextChannelActor(client, guild, channel),
    INewsChannelActor;

public partial class RestNewsChannel :
    RestTextChannel,
    INewsChannel,
    IContextConstructable<RestNewsChannel, IGuildNewsChannelModel, GuildIdentity, DiscordRestClient>
{
    internal override IGuildNewsChannelModel Model => _model;

    internal override RestNewsChannelActor ChannelActor { get; }

    private IGuildNewsChannelModel _model;

    internal RestNewsChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildNewsChannelModel model,
        RestNewsChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;

        ChannelActor = actor ?? new(client, guild, this);
    }

    public ValueTask UpdateAsync(IGuildNewsChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public static RestNewsChannel Construct(DiscordRestClient client, IGuildNewsChannelModel model, IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild)
        => new(client, guild, model);
}
