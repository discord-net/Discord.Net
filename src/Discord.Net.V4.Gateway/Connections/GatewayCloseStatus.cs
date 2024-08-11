using System.Net.WebSockets;

namespace Discord.Gateway;

public readonly record struct GatewayCloseStatus(GatewayCloseCode StatusCode, string? Reason);
