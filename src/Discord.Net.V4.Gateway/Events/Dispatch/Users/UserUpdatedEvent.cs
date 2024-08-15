using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway.Dispatch;

public sealed partial class UserUpdatedEventPackage : IDispatchPackage;

public delegate ValueTask UserUpdatedDelegate(
    [Supports(EventParameterDegree.All)] GatewayCurrentUser user
);

[Subscribable<UserUpdatedDelegate>]
[DispatchEvent(DispatchEventNames.UserUpdated)]
public sealed partial class UserUpdatedEvent(
    DiscordGatewayClient client
) :
    DispatchEvent<UserUpdatedEventPackage, IUserUpdatedPayloadData>(client)
{
    public GatewayCurrentUserActor GetUserActor(IUserUpdatedPayloadData payload) => Client.CurrentUser;

    public async ValueTask<IEntityHandle<ulong, GatewayCurrentUser>> GetUserHandleAsync(
        IUserUpdatedPayloadData payload,
        CancellationToken token)
    {
        var broker = await Brokers.CurrentUser.GetConfiguredBrokerAsync(Client, token: token);
        return await broker.CreateAsync(payload, CachePathable.Empty, Client.CurrentUser, token);
    }

    public override ValueTask<UserUpdatedEventPackage?> PackageAsync(
        IUserUpdatedPayloadData? payload,
        CancellationToken token = default
    ) => ValueTask.FromResult(
        payload is not null
            ? new UserUpdatedEventPackage(this, payload)
            : null
    );
}