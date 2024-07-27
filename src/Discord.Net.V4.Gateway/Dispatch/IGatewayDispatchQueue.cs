namespace Discord.Gateway.Events;

public interface IGatewayDispatchQueue
{
    Task AcceptAsync(string dispatchType, IGatewayPayloadData payload);
}
