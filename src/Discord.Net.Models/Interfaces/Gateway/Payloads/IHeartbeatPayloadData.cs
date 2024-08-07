namespace Discord.Models;

public interface IHeartbeatPayloadData : IGatewayPayloadData
{
    int? LastSequence { get; }
}
