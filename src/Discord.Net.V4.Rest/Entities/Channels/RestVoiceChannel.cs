using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;

namespace Discord.Rest.Channels;

[method: TypeFactory]
public partial class RestLoadableVoiceChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    VoiceChannelIdentity channel
):
    RestVoiceChannelActor(client, guild, channel),
    ILoadableVoiceChannelActor
{

    [RestLoadableActorSource]
    [ProxyInterface(typeof(ILoadableEntity<IVoiceChannel>))]
    internal RestLoadable<ulong, RestVoiceChannel, IVoiceChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestVoiceChannel, IGuildVoiceChannelModel>(
                (_, model) => RestVoiceChannel.Construct(client, model, guild)
            ).Invoke
        );
}

public partial class RestVoiceChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    VoiceChannelIdentity channel
) :
    RestGuildChannelActor(client, guild, channel),
    IVoiceChannelActor,
    IActor<ulong, RestVoiceChannel>
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, channel);

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestVoiceChannel CreateEntity(IGuildVoiceChannelModel model)
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
    internal override RestVoiceChannelActor Actor { get; }

    private IGuildVoiceChannelModel _model;

    internal RestVoiceChannel(DiscordRestClient client,
        GuildIdentity guild,
        IGuildVoiceChannelModel model,
        RestVoiceChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;

        Actor = actor ?? new(
            client,
            guild,
            VoiceChannelIdentity.Of(this)
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

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(IGuildVoiceChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IGuildVoiceChannelModel GetModel() => Model;
}
