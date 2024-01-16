using Discord.Rest;

namespace Discord.API.AuditLogs;

internal class ThreadInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("name")]
    public string Name { get; set; }

    [JsonField("type")]
    public ThreadType Type { get; set; }

    [JsonField("archived")]
    public bool? IsArchived { get; set; }

    [JsonField("locked")]
    public bool? IsLocked { get; set;}

    [JsonField("auto_archive_duration")]
    public ThreadArchiveDuration? ArchiveDuration { get; set; }

    [JsonField("rate_limit_per_user")]
    public int? SlowModeInterval { get; set; }

    [JsonField("flags")]
    public ChannelFlags? ChannelFlags { get; set; }

    [JsonField("applied_tags")]
    public ulong[] AppliedTags { get; set; }
}
