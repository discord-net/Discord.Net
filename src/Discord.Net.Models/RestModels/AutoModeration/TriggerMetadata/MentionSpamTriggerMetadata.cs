using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MentionSpamTriggerMetadata : TriggerMetadata, IMentionSpamTriggerMetadataModel
{
    [JsonPropertyName("mention_total_limit")]
    public int MentionTotalLimit { get; set; }

    [JsonPropertyName("mention_raid_protection_enabled")]
    public bool MentionRaidProtectionEnabled { get; set; }
}
