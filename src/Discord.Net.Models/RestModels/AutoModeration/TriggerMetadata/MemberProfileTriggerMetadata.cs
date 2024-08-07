using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MemberProfileTriggerMetadata : TriggerMetadata, IMemberProfileTriggerMetadataModel
{
    [JsonPropertyName("keyword_filter")]
    public required string[] KeywordFilter { get; set; }

    [JsonPropertyName("regex_patterns")]
    public required string[] RegexPatterns { get; set; }

    [JsonPropertyName("allow_list")]
    public required string[] AllowList { get; set; }
}
