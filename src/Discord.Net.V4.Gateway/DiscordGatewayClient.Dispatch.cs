using Discord.Gateway.Events;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Models.Dispatch;
using Microsoft.Extensions.Logging;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient
{
    private readonly IGatewayDispatchQueue _dispatchQueue;

    private async Task ProcessDispatchAsync(string type, IGatewayPayloadData? payload)
    {
        // TODO:
        // we update state first, then invoke the event queue
        switch (type)
        {
            case DispatchEventNames.Ready when payload is IReadyPayload readyPayload:
                await HandleReadyAsync(readyPayload);
                break;
        }


        await _dispatchQueue.AcceptAsync(type, payload);
    }

    private async Task HandleReadyAsync(IReadyPayload readyPayload)
    {
        StateController ??= new(this, LoggerFactory.CreateLogger<StateController>(), readyPayload.User);

        // TODO: shards, resume, session
    }
}
