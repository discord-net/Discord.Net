using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class StageInstancePayload : IStageInstancePayloadData
{
    [JsonIgnore, JsonExtend]
    public required StageInstance StageInstance { get; set; }
}
