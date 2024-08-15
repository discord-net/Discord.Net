namespace Discord.Gateway;

public sealed class HeartbeatUnacknowledgedException(
    int attempts
) : DiscordException($"Discord failed to acknowledge {attempts} heartbeats, the connection is zombified");