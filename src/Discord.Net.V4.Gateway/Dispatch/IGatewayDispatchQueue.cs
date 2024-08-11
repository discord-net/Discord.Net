using Discord.Models;

namespace Discord.Gateway.Events;

public interface IGatewayDispatchQueue
{
    ValueTask AcceptAsync(
        string eventName,
        HashSet<IDispatchEvent>? dispatchEvents,
        IGatewayPayloadData? payload,
        CancellationToken token
    );
}
