using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Channels;

public partial class RestLoadableVoiceChannelActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestTextChannelActor(client, guildId, id),
    ILoadableVoiceChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IVoiceChannel>))]
    internal RestLoadable<ulong, RestVoiceChannel, IVoiceChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestVoiceChannel, GuildVoiceChannel>(
                (_, model) => RestVoiceChannel.Construct(client, model, guildId)
            )
        );
}

[ExtendInterfaceDefaults(
    typeof(IModifiable<ulong, IVoiceChannelActor, ModifyVoiceChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestVoiceChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    IIdentifiableEntityOrModel<ulong, RestVoiceChannel> channel
):
    RestTextChannelActor(client, guild, channel),
    IVoiceChannelActor
{
    public IVoiceChannel CreateEntity(IGuildVoiceChannelModel model)
        => RestVoiceChannel.Construct(Client, model, Guild.Identity);
}

public partial class RestVoiceChannel :
    RestTextChannel,
    IVoiceChannel,
    IContextConstructable<RestVoiceChannel, IGuildVoiceChannelModel, GuildIdentity, DiscordRestClient>
{
    internal override IGuildVoiceChannelModel Model => _model;

    [ProxyInterface(
        typeof(IVoiceChannelActor),
        typeof(IEntityProvider<IVoiceChannel, IGuildVoiceChannelModel>)
    )]
    internal override RestVoiceChannelActor ChannelActor { get; }

    private IGuildVoiceChannelModel _model;

    internal RestVoiceChannel(DiscordRestClient client,
        GuildIdentity guild,
        IGuildVoiceChannelModel model,
        RestVoiceChannelActor? actor = null) : base(client, guild, model)
    {
        _model = model;

        ChannelActor = actor ?? new(client, guild, this);
    }

    public ValueTask UpdateAsync(IGuildVoiceChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public static RestVoiceChannel Construct(DiscordRestClient client, IGuildVoiceChannelModel model, GuildIdentity guild)
        => new(client, guild, model);

    public string? RTCRegion => Model.RTCRegion;

    public int Bitrate => Model.Bitrate;

    public int? UserLimit => Model.UserLimit;

    public VideoQualityMode VideoQualityMode => (VideoQualityMode?)Model.VideoQualityMode ?? VideoQualityMode.Auto;
}
