using System.Collections.Frozen;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Models.Dispatch;

namespace Discord.Gateway.Dispatch;

public sealed partial class ReadyPackage : IDispatchPackage<IReadyPayloadData>;

public delegate ValueTask ReadyEventDelegate(
    [Supports(EventParameterDegree.All)] [PackageSource(nameof(IReadyPayloadData.User))]
    GatewayCurrentUser currentUser,
    IReadOnlySet<ulong> guilds
);

[Subscribable<ReadyEventDelegate>]
[DispatchEvent(DispatchEventNames.Ready)]
public sealed partial class ReadyEvent(DiscordGatewayClient client) :
    DispatchEvent<ReadyPackage, IReadyPayloadData>(client)
{
    private readonly DiscordGatewayClient _client = client;

    public override ValueTask<ReadyPackage?> PackageAsync(
        IReadyPayloadData? payload,
        CancellationToken token = default)
    {
        return payload is null
            ? ValueTask.FromResult<ReadyPackage?>(null)
            : ValueTask.FromResult<ReadyPackage?>(new ReadyPackage(this, payload));
    }

    public GatewayCurrentUserActor GetCurrentUserActor(IReadyPayloadData payload)
        => _client.CurrentUser;

    public async ValueTask<IEntityHandle<ulong, GatewayCurrentUser>> GetCurrentUserHandleAsync(
        IReadyPayloadData payload, CancellationToken token)
        => await Brokers.CurrentUser
            .GetBroker(_client)
            .CreateAsync(payload.User, CachePathable.Empty, _client.CurrentUser, token);

    public ValueTask<IReadOnlySet<ulong>> GetGuildsAsync(IReadyPayloadData payload, CancellationToken token)
        => ValueTask.FromResult<IReadOnlySet<ulong>>(_client.UnavailableGuilds.ToFrozenSet());
}