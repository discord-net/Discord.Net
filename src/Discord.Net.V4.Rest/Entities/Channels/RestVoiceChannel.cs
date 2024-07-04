using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Channels;

public partial class RestLoadableVoiceChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    VoiceChannelIdentity channel
) :
    RestVoiceChannelActor(client, guild, channel),
    ILoadableVoiceChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IVoiceChannel>))]
    internal RestLoadable<ulong, RestVoiceChannel, IVoiceChannel, Channel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestVoiceChannel, GuildVoiceChannel>(
                (_, model) => RestVoiceChannel.Construct(client, model, guild)
            )
        );
}

[ExtendInterfaceDefaults(
    typeof(IModifiable<ulong, IVoiceChannelActor, ModifyVoiceChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestVoiceChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    VoiceChannelIdentity channel
) :
    RestGuildChannelActor(client, guild, channel),
    IVoiceChannelActor
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, guild, channel);

    public IVoiceChannel CreateEntity(IGuildVoiceChannelModel model)
        => RestVoiceChannel.Construct(Client, model, Guild.Identity);
}

public partial class RestVoiceChannel :
    RestGuildChannel,
    IVoiceChannel,
    IContextConstructable<RestVoiceChannel, IGuildVoiceChannelModel, GuildIdentity, DiscordRestClient>
{
    public string? RTCRegion => Model.RTCRegion;

    public int Bitrate => Model.Bitrate;

    public int? UserLimit => Model.UserLimit;

    public VideoQualityMode VideoQualityMode => (VideoQualityMode?)Model.VideoQualityMode ?? VideoQualityMode.Auto;

    internal override IGuildVoiceChannelModel Model => _model;

    [ProxyInterface(
        typeof(IVoiceChannelActor),
        typeof(IMessageChannelActor),
        typeof(IEntityProvider<IVoiceChannel, IGuildVoiceChannelModel>)
    )]
    internal override RestVoiceChannelActor ChannelActor { get; }

    private IGuildVoiceChannelModel _model;

    internal RestVoiceChannel(DiscordRestClient client,
        GuildIdentity guild,
        IGuildVoiceChannelModel model,
        RestVoiceChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;

        ChannelActor = actor ?? new(
            client,
            guild,
            this.Identity<ulong, RestVoiceChannel, IGuildVoiceChannelModel>()
        );
    }

    public static RestVoiceChannel Construct(
        DiscordRestClient client,
        IGuildVoiceChannelModel model,
        GuildIdentity guild)
    {
        switch (model)
        {
            case IGuildStageChannelModel stage:
                return RestStageChannel.Construct(client, stage, guild);
            default:
                return new(client, guild, model);
        }
    }

    public ValueTask UpdateAsync(IGuildVoiceChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IGuildVoiceChannelModel GetModel() => Model;
}
