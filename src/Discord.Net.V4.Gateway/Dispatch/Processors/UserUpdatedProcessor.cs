using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway.Processors;

[DispatchEvent(DispatchEventNames.UserUpdated)]
public sealed partial class UserUpdatedProcessor :
    IDispatchProcessor<IUserUpdatedPayloadData>
{
    public static async ValueTask ProcessAsync(DiscordGatewayClient client, IUserUpdatedPayloadData payload, CancellationToken token = default)
    {
        var store = await client.StateController.GetRootStoreAsync(Template.Of<GatewayCurrentUserActor>(), token);
        var broker = await GatewayCurrentUserActor.GetBrokerAsync(client, token);

        await broker.UpdateAsync(payload, store, token);
    }
}
