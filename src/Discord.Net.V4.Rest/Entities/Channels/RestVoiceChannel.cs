using Discord.Models;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestVoiceChannelActor :
    RestGuildChannelActor,
    IRestActor<RestVoiceChannelActor, ulong, RestVoiceChannel, IGuildVoiceChannelModel>,
    IVoiceChannelActor,
    IRestMessageChannelTrait,
    IRestIntegrationChannelTrait.WithIncoming,
    IRestGuildChannelInvitableTrait
{
    [SourceOfTruth] internal override VoiceChannelIdentity Identity { get; }

    [method: TypeFactory]
    public RestVoiceChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        VoiceChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestVoiceChannel CreateEntity(IGuildVoiceChannelModel model)
        => RestVoiceChannel.Construct(Client, this, model);
}

[ExtendInterfaceDefaults]
public partial class RestVoiceChannel :
    RestGuildChannel,
    IVoiceChannel,
    IRestConstructable<RestVoiceChannel, RestVoiceChannelActor, IGuildVoiceChannelModel>,
    IRestNestedChannelTrait
{
    public string? RTCRegion => Model.RTCRegion;

    public int Bitrate => Model.Bitrate;

    public int? UserLimit => Model.UserLimit;

    public VideoQualityMode VideoQualityMode => (VideoQualityMode?) Model.VideoQualityMode ?? VideoQualityMode.Auto;

    internal override IGuildVoiceChannelModel Model => _model;

    [ProxyInterface(
        typeof(IVoiceChannelActor),
        typeof(IMessageChannelTrait),
        typeof(IEntityProvider<IVoiceChannel, IGuildVoiceChannelModel>)
    )]
    internal override RestVoiceChannelActor Actor { get; }

    private IGuildVoiceChannelModel _model;

    internal RestVoiceChannel(
        DiscordRestClient client,
        IGuildVoiceChannelModel model,
        RestVoiceChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;
    }

    public static RestVoiceChannel Construct(
        DiscordRestClient client,
        RestVoiceChannelActor actor,
        IGuildVoiceChannelModel model)
    {
        switch (model)
        {
            case IGuildStageChannelModel stage:
                return RestStageChannel.Construct(
                    client,
                    actor as RestStageChannelActor ?? actor.Guild.Channels.Stage[actor.Id],
                    stage
                );
            default:
                return new(client, model, actor);
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