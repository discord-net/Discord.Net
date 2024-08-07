namespace Discord.Models;

public interface IMessageDeletePayloadData : IGatewayPayloadData
{
    ulong Id { get; }
    ulong ChannelId { get; }
    ulong? GuildId { get; }
}
