namespace Discord.Models;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
internal sealed class GatewayPayloadData(GatewayOpCode opCode, string? eventType = null) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.

public interface IGatewayPayloadData;
