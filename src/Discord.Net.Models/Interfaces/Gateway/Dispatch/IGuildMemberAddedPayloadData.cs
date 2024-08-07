namespace Discord.Models;

public interface IGuildMemberAddedPayloadData : IGatewayPayloadData, IMemberModel
{
    ulong GuildId { get; }
}
