namespace Discord.Models;

public interface IGatewayMessage
{
    GatewayOpCode OpCode { get; }
    IGatewayPayloadData? Payload { get; }
    int? Sequence { get; }
    string? EventName { get; }
}
