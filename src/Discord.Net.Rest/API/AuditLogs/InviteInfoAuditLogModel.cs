using Discord.Rest;

namespace Discord.API.AuditLogs;

internal class InviteInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("code")]
    public string Code { get; set; }

    [JsonField("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonField("inviter_id")]
    public ulong? InviterId { get; set; }

    [JsonField("uses")]
    public int? Uses { get; set; }

    [JsonField("max_uses")]
    public int? MaxUses { get; set; }

    [JsonField("max_age")]
    public int? MaxAge { get; set; }

    [JsonField("temporary")]
    public bool? Temporary { get; set; }
}
