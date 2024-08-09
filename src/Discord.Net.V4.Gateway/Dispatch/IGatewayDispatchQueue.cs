using Discord.Models;

namespace Discord.Gateway.Events;

public interface IGatewayDispatchQueue
{
    ValueTask AcceptAsync(string eventName, IDispatchEvent? dispatchEvent, IGatewayPayloadData? payload, CancellationToken token);
}
