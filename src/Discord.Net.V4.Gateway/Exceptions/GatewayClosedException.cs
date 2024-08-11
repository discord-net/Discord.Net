namespace Discord.Gateway;

public sealed class GatewayClosedException(GatewayCloseStatus status)
    : DiscordException("The gateway connection was closed")
{
    public GatewayCloseStatus Status { get; } = status;
}
