namespace Discord.Models;

[ModelEquality]
public partial interface IGatewayMessage : IModel
{
    GatewayOpCode OpCode { get; }
    IGatewayPayloadData? Payload { get; }
    int? Sequence { get; }
    string? EventName { get; }
}
