using Discord.Gateway.State;
using Discord.Models;
using Discord.Stage;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayStageChannelActor(
    DiscordGatewayClient client,
    GuildIdentity guild,
    StageChannelIdentity channel
) :
    GatewayVoiceChannelActor(client, guild, channel),
    IStageChannelActor,
    IGatewayCachedActor<ulong, GatewayStageChannel, StageChannelIdentity, IGuildStageChannelModel>
{
    [SourceOfTruth] internal override StageChannelIdentity Identity { get; } = channel;

    [SourceOfTruth]
    public GatewayStageInstanceActor StageInstance { get; }
        = new(client, guild, channel, StageInstanceIdentity.Of(channel.Id));

    [SourceOfTruth]
    internal GatewayStageChannel CreateEntity(IGuildStageChannelModel model)
        => Client.StateController.CreateLatent(this, model);

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
        ICacheConstructionContext context,
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
