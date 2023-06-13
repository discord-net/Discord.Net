using Newtonsoft.Json;

namespace Discord.API;

internal class AuditLogOptions
{
    [JsonProperty("count")]
    public int? Count { get; set; }
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }
    [JsonProperty("message_id")]
    public ulong? MessageId { get; set; }

    //Prune
    [JsonProperty("delete_member_days")]
    public int? PruneDeleteMemberDays { get; set; }
    [JsonProperty("members_removed")]
    public int? PruneMembersRemoved { get; set; }

    //Overwrite Update
    [JsonProperty("role_name")]
    public string OverwriteRoleName { get; set; }
    [JsonProperty("type")]
    public PermissionTarget OverwriteType { get; set; }
    [JsonProperty("id")]
    public ulong? OverwriteTargetId { get; set; }

    // App command perm update
    [JsonProperty("application_id")]
    public ulong? ApplicationId { get; set; }

    // Automod

    [JsonProperty("auto_moderation_rule_name")]
    public string AutoModRuleName { get; set; }

    [JsonProperty("auto_moderation_rule_trigger_type")]
    public AutoModTriggerType? AutoModRuleTriggerType { get; set; }
}
