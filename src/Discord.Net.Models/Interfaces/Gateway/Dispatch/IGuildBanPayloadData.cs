namespace Discord.Models;

public interface IGuildBanPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    IUserModel User { get; }
}
