namespace Discord.Models;

public interface IPresenceUpdatedPayloadData : IGatewayPayloadData
{
    IUserModel User { get; }
    ulong GuildId { get; }
    string Status { get; }
    IEnumerable<IActivityModel> Activities { get; }
    IClientStatusModel ClientStatus { get; }
}
