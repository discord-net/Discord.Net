using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway.Dispatch;

public sealed partial class GuildCreatedPackage : IDispatchPackage<IExtendedGuild>;

public delegate ValueTask GuildCreatedDelegate(
    [Supports(EventParameterDegree.All)] GatewayGuild guild
);

[Subscribable<GuildCreatedDelegate>]
[DispatchEvent(DispatchEventNames.GuildCreate)]
public sealed partial class GuildCreatedEvent(DiscordGatewayClient client) :
    DispatchEvent<GuildCreatedPackage, IGuildCreatePayloadData>(client)
{
    public override ValueTask<GuildCreatedPackage?> PackageAsync(
        IGuildCreatePayloadData? payload,
        CancellationToken token = default)
    {
        if (payload is not IExtendedGuild extendedGuild)
            return ValueTask.FromResult<GuildCreatedPackage?>(null);

        // TODO: sync with guild available event
        if (Client.UnavailableGuilds.Contains(payload.Id))
            return ValueTask.FromResult<GuildCreatedPackage?>(null);

        return ValueTask.FromResult<GuildCreatedPackage?>(
            new GuildCreatedPackage(this, extendedGuild)
        );
    }

    public GatewayGuildActor GetGuildActor(IExtendedGuild payload) => Client.Guilds[payload.Id];

    public async ValueTask<IEntityHandle<ulong, GatewayGuild>> GetGuildHandleAsync(
        IExtendedGuild payload,
        CancellationToken token)
    {
        var broker = await Brokers.Guild.GetConfiguredBrokerAsync(Client, token: token);
        return await broker.CreateAsync(payload, CachePathable.Empty, token);
    }
}
