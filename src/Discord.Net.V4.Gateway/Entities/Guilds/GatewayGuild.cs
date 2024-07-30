using Discord.Gateway.Cache;
using Discord.Models;

namespace Discord.Gateway.Guilds;

public sealed partial class GatewayGuildActor :
    GatewayCachedActor<ulong, GatewayGuild, GuildIdentity, IGuildModel>,
    IGuildActor
{
    public GatewayGuildActor(DiscordGatewayClient client, GuildIdentity guild)
        : base(client, guild)
    {

    }

    protected override ValueTask<IEntityModelStore<ulong, IGuildModel>> GetStoreAsync(CancellationToken token = default)
        => Client.StateController
            .GetStoreAsync(Template.Of<GuildIdentity>(), token);
}

public sealed class GatewayGuild :
    GatewayCacheableEntity<GatewayGuild, ulong, IGuildModel, GuildIdentity>,
    IGuild,
    IContextConstructable<GatewayGuild, IGuildModel, IPathable, DiscordGatewayClient>
{

}
