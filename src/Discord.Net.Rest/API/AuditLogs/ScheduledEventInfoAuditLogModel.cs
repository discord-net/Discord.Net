using Discord.Rest;
using System;

namespace Discord.API.AuditLogs;

internal class ScheduledEventInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("channel_id")]
    public ulong? ChannelId { get; set; }
    
    [JsonField("name")]
    public string Name { get; set; }

    [JsonField("description")]
    public string Description { get; set; }

    [JsonField("scheduled_start_time")]
    public DateTimeOffset? StartTime { get; set; }

    [JsonField("scheduled_end_time")]
    public DateTimeOffset? EndTime { get; set; }

    [JsonField("privacy_level")]
    public GuildScheduledEventPrivacyLevel? PrivacyLevel { get; set; }

    [JsonField("status")]
    public GuildScheduledEventStatus? EventStatus { get; set; }

    [JsonField("entity_type")]
    public GuildScheduledEventType? EventType { get; set; }

    [JsonField("entity_id")]
    public ulong? EntityId { get; set; }
    
    [JsonField("user_count")]
    public int? UserCount { get; set; }

    [JsonField("location")]
    public string Location { get; set; }

    [JsonField("image")]
    public string Image { get; set; }
}
