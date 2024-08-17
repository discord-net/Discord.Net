using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class HeartbeatPayloadData : IHeartbeatPayloadData
{
    [JsonIgnore, JsonExtend]
    public int? LastSequence { get; set; }
}
