namespace Discord.Models;

public interface IBulkMessageDeletePayloadData : IGatewayPayloadData
{
    ulong[] Ids { get; }
    ulong ChannelId { get; }
    ulong? GuildId { get; }
}
