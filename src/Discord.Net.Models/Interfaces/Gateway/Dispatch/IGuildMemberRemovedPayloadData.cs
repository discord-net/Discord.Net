namespace Discord.Models;

public interface IGuildMemberRemovedPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    IUserModel User { get; }
}
