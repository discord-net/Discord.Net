using Discord.Gateway.State;
using Discord.Models;
using Discord.Stage;
using static Discord.Template;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewayStageChannelActor :
    GatewayVoiceChannelActor,
    IStageChannelActor,
    IGatewayCachedActor<ulong, GatewayStageChannel, StageChannelIdentity, IGuildStageChannelModel>
{
    [SourceOfTruth]
    public GatewayStageInstanceActor StageInstance { get; }

    [SourceOfTruth] internal override StageChannelIdentity Identity { get; }

    [TypeFactory]
    public GatewayStageChannelActor(DiscordGatewayClient client,
        GuildIdentity guild,
        StageChannelIdentity channel) : base(client, guild, channel)
    {
        Identity = channel | this;
        StageInstance = new GatewayStageInstanceActor(client, guild, channel, StageInstanceIdentity.Of(channel.Id));
    }

    [SourceOfTruth]
    internal GatewayStageChannel CreateEntity(IGuildStageChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);

    [SourceOfTruth]
    internal GatewayStageInstance CreateEntity(IStageInstanceModel model)
        => Client.StateController
            .CreateLatent<ulong, GatewayStageInstance, GatewayStageInstanceActor, IStageInstanceModel>(
                model,
                CachePath
            );
}

public sealed partial class GatewayStageChannel :
    GatewayVoiceChannel,
    IStageChannel,
    ICacheableEntity<GatewayStageChannel, ulong, IGuildStageChannelModel>
{
    [ProxyInterface]
    internal override GatewayStageChannelActor Actor { get; }

    internal override IGuildStageChannelModel Model => _model;

    private IGuildStageChannelModel _model;

    public GatewayStageChannel(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IGuildStageChannelModel model,
        GatewayStageChannelActor? actor = null
    ) : base(client, guild, model, actor)
    {
        _model = model;
        Actor = actor ?? new(client, guild, StageChannelIdentity.Of(this));
    }

    public static GatewayStageChannel Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IGuildStageChannelModel model
    ) => new(
        client,
        context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
        model,
        context.TryGetActor<GatewayStageChannelActor>()
    );

    [SourceOfTruth]
    public ValueTask UpdateAsync(
        IGuildStageChannelModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        _model = model;

        return base.UpdateAsync(model, false, token);
    }

    public override IGuildStageChannelModel GetModel() => Model;
}
