using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

public sealed partial class GatewayCategoryChannelActor(
    DiscordGatewayClient client,
    GuildIdentity guild,
    CategoryChannelIdentity channel
):
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
    [ProxyInterface]
    internal override GatewayCategoryChannelActor Actor { get; }

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

    public static GatewayCategoryChannel Construct(DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewayCategoryChannel> context, IGuildCategoryChannelModel model) =>
        throw new NotImplementedException();

    public IGuildCategoryChannelModel GetModel() => throw new NotImplementedException();

    public ValueTask UpdateAsync(IGuildCategoryChannelModel model, bool updateCache = true, CancellationToken token = default) => throw new NotImplementedException();
}
