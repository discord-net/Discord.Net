namespace Discord.Gateway;

public sealed class UnexpectedGatewayPayloadException(Type expected, IGatewayPayloadData? actual)
    : DiscordException($"Expected payload type of {expected}, got {actual?.GetType().Name ?? "null"}");
