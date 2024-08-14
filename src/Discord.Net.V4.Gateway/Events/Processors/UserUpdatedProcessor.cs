using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway.Processors;

[DispatchEvent(DispatchEventNames.UserUpdated)]
public sealed partial class UserUpdatedProcessor(DiscordGatewayClient client) :
    IDispatchProcessor<IUserUpdatedPayloadData>
{
    public async ValueTask ProcessAsync(
        IUserUpdatedPayloadData payload,
        CancellationToken token = default)
    {
        var broker = await GatewayCurrentUserActor.GetConfiguredBrokerAsync(client, CachePathable.Empty, token);
        await broker.UpdateAsync(payload, token);
    }
}
