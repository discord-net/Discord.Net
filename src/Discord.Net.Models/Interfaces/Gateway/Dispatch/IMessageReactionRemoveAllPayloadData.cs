namespace Discord.Models;

public interface IMessageReactionRemoveAllPayloadData : IGatewayPayloadData
{
    ulong ChannelId { get; }
    ulong MessageId { get; }
    ulong? GuildId { get; }
}
