namespace Discord.Models;

public interface IMessagePollVotePayloadData : IGatewayPayloadData
{
    ulong UserId { get; }
    ulong ChannelId { get; }
    ulong MessageId { get; }
    ulong? GuildId { get; }
    ulong AnswerId { get; }
}
