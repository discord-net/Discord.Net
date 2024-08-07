namespace Discord.Models.Json;

public sealed class HeartbeatPayloadData : IHeartbeatPayloadData
{
    public int? LastSequence { get; set; }
}
