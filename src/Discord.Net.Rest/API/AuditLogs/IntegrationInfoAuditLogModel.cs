using Discord.Rest;

namespace Discord.API.AuditLogs;

internal class IntegrationInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("name")]
    public string Name { get; set; }

    [JsonField("type")]
    public string Type { get; set; }

    [JsonField("enabled")]
    public bool? Enabled { get; set; }

    [JsonField("syncing")]
    public bool? Syncing { get; set; }

    [JsonField("role_id")]
    public ulong? RoleId { get; set; }

    [JsonField("enable_emoticons")]
    public bool? EnableEmojis { get; set; }

    [JsonField("expire_behavior")]
    public IntegrationExpireBehavior? ExpireBehavior { get; set; }

    [JsonField("expire_grace_period")]
    public int? ExpireGracePeriod { get; set; }

    [JsonField("scopes")]
    public string[] Scopes { get; set; }
}
