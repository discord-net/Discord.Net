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
        var store = await client.StateController.GetRootStoreAsync(Template.Of<GatewayCurrentUserActor>(), token);
        var broker = await GatewayCurrentUserActor.GetBrokerAsync(client, token);

        await broker.UpdateAsync(payload, store, token);
    }
}
