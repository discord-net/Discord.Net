namespace Discord.Models;

public interface ITypingStartPayloadData : IGatewayPayloadData
{
    ulong ChannelId { get; }
    ulong? GuildId { get; }
    ulong UserId { get; }
    int Timestamp { get; }
    IMemberModel? Member { get; }
}
