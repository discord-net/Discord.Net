using Discord.Rest;

namespace Discord.API.AuditLogs;

internal class WebhookInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonField("name")]
    public string Name { get; set; }

    [JsonField("type")]
    public WebhookType? Type { get; set; }

    [JsonField("avatar_hash")]
    public string AvatarHash { get; set; }
}
