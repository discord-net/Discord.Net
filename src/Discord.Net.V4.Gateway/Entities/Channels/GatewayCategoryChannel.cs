using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

[method: TypeFactory]
public sealed partial class GatewayCategoryChannelActor(
    DiscordGatewayClient client,
    GuildIdentity guild,
    CategoryChannelIdentity channel
) :
    GatewayGuildChannelActor(client, guild, channel),
    ICategoryChannelActor,
    IGatewayCachedActor<ulong, GatewayCategoryChannel, CategoryChannelIdentity, IGuildCategoryChannelModel>
{
    [SourceOfTruth] internal override CategoryChannelIdentity Identity { get; } = channel;

    [SourceOfTruth]
    internal GatewayCategoryChannel CreateEntity(IGuildCategoryChannelModel model)
        => Client.StateController.CreateLatent(this, model);
}

public sealed partial class GatewayCategoryChannel :
    GatewayGuildChannel,
    ICategoryChannel,
    ICacheableEntity<GatewayCategoryChannel, ulong, IGuildCategoryChannelModel>
{
    [ProxyInterface] internal override GatewayCategoryChannelActor Actor { get; }

    internal override IGuildCategoryChannelModel Model => _model;

    private IGuildCategoryChannelModel _model;

    public GatewayCategoryChannel(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IGuildCategoryChannelModel model,
        GatewayCategoryChannelActor? actor = null,
        IEntityHandle<ulong, GatewayCategoryChannel>? implicitHandle = null
    ) : base(client, guild, model, actor, implicitHandle)
    {
        _model = model;
        Actor = actor ?? new(client, guild, CategoryChannelIdentity.Of(this));
    }

    public static GatewayCategoryChannel Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewayCategoryChannel> context,
        IGuildCategoryChannelModel model
    ) => new(
        client,
        context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
        model,
        context.TryGetActor(T<GatewayCategoryChannelActor>()),
        context.ImplicitHandle
    );

    [CovariantOverride]
    public ValueTask UpdateAsync(
        IGuildCategoryChannelModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        _model = model;

        return base.UpdateAsync(model, false, token);
    }

    public override IGuildCategoryChannelModel GetModel() => Model;
}
