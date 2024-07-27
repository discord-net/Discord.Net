using Discord.Gateway.Events;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient
{
    private readonly IGatewayDispatchQueue _dispatchQueue;

    private async Task ProcessDispatchAsync(string type, IGatewayPayloadData payload)
    {
        // TODO:
        // we update state first, then invoke the event queue


        await _dispatchQueue.AcceptAsync(type, payload);
    }
}
