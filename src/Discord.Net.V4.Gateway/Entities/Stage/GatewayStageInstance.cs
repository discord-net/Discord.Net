using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using Discord;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayStageInstanceActor :
    GatewayCachedActor<ulong, GatewayStageInstance, StageInstanceIdentity, IStageInstanceModel>,
    IStageInstanceActor
{
    [SourceOfTruth, StoreRoot] public GatewayStageChannelActor Channel { get; }

    [SourceOfTruth] public GatewayGuildActor Guild { get; }

    internal override StageInstanceIdentity Identity { get; }

    public GatewayStageInstanceActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        StageChannelIdentity channel,
        StageInstanceIdentity instance
    ) : base(client, instance)
    {
        Identity = instance | this;

        Guild = client.Guilds[guild];
        Channel = Guild.StageChannels[channel];
    }

    [SourceOfTruth]
    internal GatewayStageInstance CreateEntity(IStageInstanceModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayStageInstance :
    GatewayCacheableEntity<GatewayStageInstance, ulong, IStageInstanceModel>,
    IStageInstance
{
    public string Topic => Model.Topic;

    public StagePrivacyLevel PrivacyLevel => (StagePrivacyLevel)Model.PrivacyLevel;

    public IGuildScheduledEventActor? Event => throw new NotImplementedException();

    [ProxyInterface] internal GatewayStageInstanceActor Actor { get; }

    internal IStageInstanceModel Model { get; private set; }

    public GatewayStageInstance(
        DiscordGatewayClient client,
        GuildIdentity guild,
        StageChannelIdentity channel,
        IStageInstanceModel model,
        GatewayStageInstanceActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, channel, StageInstanceIdentity.Of(this));
    }

    public static GatewayStageInstance Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IStageInstanceModel model
    ) => new(
        client,
        context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
        context.Path.GetIdentity(T<StageChannelIdentity>(), model.ChannelId),
        model,
        context.TryGetActor<GatewayStageInstanceActor>()
    );

    public override ValueTask UpdateAsync(
        IStageInstanceModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IStageInstanceModel GetModel() => Model;
}
