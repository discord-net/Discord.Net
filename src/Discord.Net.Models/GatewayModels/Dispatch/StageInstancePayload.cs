using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class StageInstancePayload : IStageInstancePayloadData
{
    [JsonIgnore, JsonExtend] public StageInstance StageInstance { get; set; } = null!;
}
