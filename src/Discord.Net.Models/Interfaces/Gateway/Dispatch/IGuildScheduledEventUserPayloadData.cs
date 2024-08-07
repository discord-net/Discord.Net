namespace Discord.Models;

public interface IGuildScheduledEventUserPayloadData : IGatewayPayloadData
{
    ulong GuildScheduledEventId { get; }
    ulong GuildId { get; }
    ulong UserId { get; }
}
