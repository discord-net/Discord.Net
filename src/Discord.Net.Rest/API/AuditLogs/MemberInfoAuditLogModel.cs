using Discord.Rest;
using System;

namespace Discord.API.AuditLogs;

internal class MemberInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("nick")]
    public string Nickname { get; set; }

    [JsonField("mute")]
    public bool? IsMuted { get; set; }

    [JsonField("deaf")]
    public bool? IsDeafened { get; set; }

    [JsonField("communication_disabled_until")]
    public DateTimeOffset? TimeOutUntil { get; set; }
}
