using Newtonsoft.Json;

namespace Discord.API;

internal class TriggerMetadata
{
    [JsonProperty("keyword_filter")]
    public Optional<string[]> KeywordFilter { get; set; }

    [JsonProperty("regex_patterns")]
    public Optional<string[]> RegexPatterns { get; set; }

    [JsonProperty("presets")]
    public Optional<KeywordPresetTypes[]> Presets { get; set; }

    [JsonProperty("allow_list")]
    public Optional<string[]> AllowList { get; set; }

    [JsonProperty("mention_total_limit")]
    public Optional<int> MentionLimit { get; set; }

    [JsonProperty("mention_raid_protection_enabled")]
    public Optional<bool> MentionRaidProtectionEnabled { get; set; }
}
