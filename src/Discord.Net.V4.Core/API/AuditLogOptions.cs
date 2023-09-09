using System.Text.Json.Serialization;

namespace Discord.API;

public class AuditLogOptions
{
    [JsonPropertyName("count")]
    public Optional<int> Count { get; set; }

    [JsonPropertyName("channel_id")]
    public Optional<ulong> ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public Optional<ulong> MessageId { get; set; }

    //Prune
    [JsonPropertyName("delete_member_days")]
    public Optional<int> PruneDeleteMemberDays { get; set; }

    [JsonPropertyName("members_removed")]
    public Optional<int> PruneMembersRemoved { get; set; }

    //Overwrite
    [JsonPropertyName("role_name")]
    public Optional<string> OverwriteRoleName { get; set; }

    [JsonPropertyName("type")]
    public Optional<PermissionTarget> OverwriteType { get; set; }

    [JsonPropertyName("id")]
    public Optional<ulong> OverwriteTargetId { get; set; }

    // App command perm update
    [JsonPropertyName("application_id")]
    public Optional<ulong> ApplicationId { get; set; }

    // AutoMod
    [JsonPropertyName("auto_moderation_rule_name")]
    public Optional<string> AutoModRuleName { get; set; }

    [JsonPropertyName("auto_moderation_rule_trigger_type")]
    public Optional<AutoModTriggerType> AutoModRuleTriggerType { get; set; }
}
