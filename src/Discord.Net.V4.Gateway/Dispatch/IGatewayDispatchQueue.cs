using Discord.Models;

namespace Discord.Gateway.Events;

public interface IGatewayDispatchQueue
{
    ValueTask AcceptAsync(string dispatchType, IGatewayPayloadData? payload, CancellationToken token);
}
