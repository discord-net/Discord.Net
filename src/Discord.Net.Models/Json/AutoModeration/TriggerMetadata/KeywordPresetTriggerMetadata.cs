using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class KeywordPresetTriggerMetadata : TriggerMetadata, IKeywordPresetTriggerMetadataModel
{
    [JsonPropertyName("presets")] public required int[] Presets { get; set; }

    [JsonPropertyName("allow_list")] public required string[] AllowList { get; set; }
}
