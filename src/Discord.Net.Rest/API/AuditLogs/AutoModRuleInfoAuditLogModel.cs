using Discord.Rest;

namespace Discord.API.AuditLogs;

internal class AutoModRuleInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("name")]
    public string Name { get; set; }
    
    [JsonField("event_type")]
    public AutoModEventType EventType { get; set; }

    [JsonField("trigger_type")]
    public AutoModTriggerType TriggerType { get; set; }

    [JsonField("trigger_metadata")]
    public TriggerMetadata TriggerMetadata { get; set; }

    [JsonField("actions")]
    public AutoModAction[] Actions { get; set; }

    [JsonField("enabled")]
    public bool Enabled { get; set; }

    [JsonField("exempt_roles")]
    public ulong[] ExemptRoles { get; set; }

    [JsonField("exempt_channels")]
    public ulong[] ExemptChannels { get; set; }
}
