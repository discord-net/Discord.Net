namespace Discord.Models;

[GatewayPayloadData(GatewayOpCode.Hello)]
public interface IHelloPayloadData : IGatewayPayloadData
{
    int HeartbeatInterval { get; }
}
