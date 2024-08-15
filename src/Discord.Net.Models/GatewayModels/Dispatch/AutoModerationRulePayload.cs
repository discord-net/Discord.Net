using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class AutoModerationRulePayload : IAutoModerationRulePayloadData
{
    [JsonIgnore, JsonExtend] public AutoModerationRule AutoModerationRule { get; set; } = null!;
}
