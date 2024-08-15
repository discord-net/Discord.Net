namespace Discord.Gateway;

public sealed class GatewayClosedException(GatewayReadResult result)
    : DiscordException("The gateway connection was closed")
{
    public GatewayReadResult Result { get; } = result;
}
