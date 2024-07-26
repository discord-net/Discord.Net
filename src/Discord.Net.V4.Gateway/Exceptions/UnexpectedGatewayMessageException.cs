namespace Discord.Gateway;

public sealed class UnexpectedGatewayMessageException : DiscordException
{
    internal UnexpectedGatewayMessageException(GatewayOpCode expected, GatewayOpCode actual)
        : base($"Expected Gateway OpCode {expected}, but got {actual}")
    {

    }
}
