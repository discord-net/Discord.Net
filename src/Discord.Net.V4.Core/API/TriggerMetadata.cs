using System.Text.Json.Serialization;

namespace Discord.API;

public class TriggerMetadata
{
    [JsonPropertyName("keyword_filter")]
    public Optional<string[]> KeywordFilter { get; set; }

    [JsonPropertyName("regex_patterns")]
    public Optional<string[]> RegexPatterns { get; set; }

    [JsonPropertyName("presets")]
    public Optional<KeywordPresetTypes[]> Presets { get; set; }

    [JsonPropertyName("allow_list")]
    public Optional<string[]> AllowList { get; set; }

    [JsonPropertyName("mention_total_limit")]
    public Optional<int> MentionLimit { get; set; }

    [JsonPropertyName("mention_raid_protection_enabled")]
    public Optional<bool> MentionRaidProtectionEnabled { get; set; }
}
