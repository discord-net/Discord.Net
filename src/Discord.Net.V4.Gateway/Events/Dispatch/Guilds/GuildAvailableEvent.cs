using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway.Dispatch;

public sealed partial class GuildAvailablePackage : IDispatchPackage<IExtendedGuild>;

public delegate ValueTask GuildAvailableDelegate(
    [Supports(EventParameterDegree.All)] GatewayGuild guild
);

[Subscribable<GuildAvailableDelegate>]
[DispatchEvent(DispatchEventNames.GuildCreate)]
public sealed partial class GuildAvailableEvent(DiscordGatewayClient client) :
    DispatchEvent<GuildAvailablePackage, IGuildCreatePayloadData>(client)
{
    public override ValueTask<GuildAvailablePackage?> PackageAsync(
        IGuildCreatePayloadData? payload,
        CancellationToken token = default)
    {
        if (payload is not IExtendedGuild guild) return ValueTask.FromResult<GuildAvailablePackage?>(null);

        // TODO: sync with guild create event
        return !Client.UnavailableGuilds.Remove(payload.Id)
            ? ValueTask.FromResult<GuildAvailablePackage?>(null)
            : ValueTask.FromResult<GuildAvailablePackage?>(new GuildAvailablePackage(this, guild));
    }

    public GatewayGuildActor GetGuildActor(IExtendedGuild payload)
        => Client.Guilds[payload.Id];

    public async ValueTask<IEntityHandle<ulong, GatewayGuild>> GetGuildHandleAsync(
        IExtendedGuild payload,
        CancellationToken token)
    {
        var broker = await Brokers.Guild.GetConfiguredBrokerAsync(Client, token: token);
        return await broker.CreateAsync(payload, CachePathable.Empty, token);
    }
}