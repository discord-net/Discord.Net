using System.Net.WebSockets;

namespace Discord.Gateway;

public readonly record struct GatewayReadResult(
    TransportFormat Format,
    GatewayCloseCode? CloseStatusCode = null,
    string? Reason = null
);