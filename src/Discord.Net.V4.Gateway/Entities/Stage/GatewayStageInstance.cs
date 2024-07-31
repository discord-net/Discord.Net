using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Stage;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayStageInstanceActor(
    DiscordGatewayClient client,
    GuildIdentity guild,
    StageChannelIdentity channel,
    StageInstanceIdentity instance
) :
    GatewayCachedActor<ulong, GatewayStageInstance, StageInstanceIdentity, IStageInstanceModel>(client, instance),
    IStageInstanceActor
{
    [SourceOfTruth]
    public GatewayStageChannelActor Channel { get; } = channel.Actor ?? new(client, guild, channel);

    [SourceOfTruth]
    public GatewayGuildActor Guild { get; } = guild.Actor ?? new(client, guild);

    [SourceOfTruth]
    internal GatewayStageInstance CreateEntity(IStageInstanceModel model)
        => Client.StateController.CreateLatent(this, model);
}

public sealed partial class GatewayStageInstance :
    GatewayCacheableEntity<GatewayStageInstance, ulong, IStageInstanceModel, StageInstanceIdentity>,
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
        GatewayStageInstanceActor? actor = null,
        IEntityHandle<ulong, GatewayStageInstance>? implicitHandle = null
    ) : base(client, model.Id, implicitHandle)
    {
        Model = model;
        Actor = actor ?? new(client, guild, channel, StageInstanceIdentity.Of(this));
    }

    public static GatewayStageInstance Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewayStageInstance> context,
        IStageInstanceModel model
    ) => new(
            client,
            context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
            context.Path.GetIdentity(T<StageChannelIdentity>(), model.ChannelId),
            model,
            context.TryGetActor(T<GatewayStageInstanceActor>()),
            context.ImplicitHandle
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
