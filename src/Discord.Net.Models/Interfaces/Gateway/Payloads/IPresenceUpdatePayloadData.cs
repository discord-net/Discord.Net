namespace Discord.Models;

public interface IPresenceUpdatePayloadData : IGatewayPayloadData
{
    int? Since { get; }
    IEnumerable<IActivityModel> Activities { get; }
    string Status { get; }
    bool IsAfk { get; }
}
