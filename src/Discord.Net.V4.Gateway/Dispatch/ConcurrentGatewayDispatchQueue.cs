namespace Discord.Gateway.Events;


public sealed partial class ConcurrentGatewayDispatchQueue : IGatewayDispatchQueue
{
    [TypeFactory]
    public ConcurrentGatewayDispatchQueue(DiscordGatewayClient client, DiscordGatewayConfig config)
    {

    }

    public Task AcceptAsync(string dispatchType, IGatewayPayloadData payload)
    {
        
    }
}
