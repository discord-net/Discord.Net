namespace Discord.Models;

public interface IHelloPayloadData : IGatewayPayloadData
{
    int HeartbeatInterval { get; }
}
