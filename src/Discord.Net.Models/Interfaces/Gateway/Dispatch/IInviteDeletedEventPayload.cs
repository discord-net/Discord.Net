namespace Discord.Models;

public interface IInviteDeletedEventPayload : IGatewayPayloadData
{
    ulong ChannelId { get; }
    ulong? GuildId { get; }
    string Code { get; }
}
