using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayVoiceChannelActor :
    GatewayGuildChannelActor,
    IVoiceChannelActor,
    IGatewayCachedActor<ulong, GatewayVoiceChannel, VoiceChannelIdentity, IGuildVoiceChannelModel>
{
    [SourceOfTruth] internal override VoiceChannelIdentity Identity { get; }

    [ProxyInterface] internal GatewayMessageChannelTrait MessageChannelActor { get; }

    [ProxyInterface]
    internal GatewayIntegrationChannelTrait IntegrationChannelActor { get; }

    [TypeFactory]
    public GatewayVoiceChannelActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        VoiceChannelIdentity channel) : base(client, guild, channel)
    {
        Identity = channel | this;
        MessageChannelActor = new GatewayMessageChannelTrait(client, channel, guild);
        IntegrationChannelActor = new GatewayIntegrationChannelTrait(client, guild, channel);
    }

    [SourceOfTruth]
    internal GatewayVoiceChannel CreateEntity(IGuildVoiceChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public partial class GatewayVoiceChannel :
    GatewayGuildChannel,
    IVoiceChannel,
    ICacheableEntity<GatewayVoiceChannel, ulong, IGuildVoiceChannelModel>
{
    [SourceOfTruth]
    public GatewayCategoryChannelActor? Category { get; private set; }

    public string? RTCRegion => Model.RTCRegion;

    public int Bitrate => Model.Bitrate;

    public int? UserLimit => Model.UserLimit;

    public VideoQualityMode VideoQualityMode => (VideoQualityMode?)Model.VideoQualityMode ?? VideoQualityMode.Auto;

    [ProxyInterface]
    internal override GatewayVoiceChannelActor Actor { get; }

    internal override IGuildVoiceChannelModel Model => _model;

    private IGuildVoiceChannelModel _model;

    public GatewayVoiceChannel(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IGuildVoiceChannelModel model,
        GatewayVoiceChannelActor? actor = null
    ) : base(client, guild, model, actor)
    {
        _model = model;
        Actor = actor ?? new(client, guild, VoiceChannelIdentity.Of(this));

        Category = model.ParentId.Map(
            (id, client, guild) => new GatewayCategoryChannelActor(client, guild, CategoryChannelIdentity.Of(id)),
            client,
            guild
        );
    }

    public static GatewayVoiceChannel Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IGuildVoiceChannelModel model)
    {
        switch (model)
        {
            case IGuildStageChannelModel guildStageChannelModel:
                return GatewayStageChannel.Construct(client, context, guildStageChannelModel);
            default:
                return new GatewayVoiceChannel(
                    client,
                    context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
                    model,
                    context.TryGetActor<GatewayVoiceChannelActor>()
                );
        }
    }

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(
        IGuildVoiceChannelModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Category = Category.UpdateFrom(
            model.ParentId,
            GatewayCategoryChannelActor.Factory,
            Client,
            Actor.Guild.Identity
        );

        _model = model;

        return base.UpdateAsync(model, false, token);
    }

    public override IGuildVoiceChannelModel GetModel() => Model;
}
