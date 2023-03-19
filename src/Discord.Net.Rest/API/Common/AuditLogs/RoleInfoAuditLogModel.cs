using Discord.Rest;

namespace Discord.API.AuditLogs;

internal class RoleInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("name")]
    public string Name { get; set; }

    [JsonField("color")]
    public uint? Color { get; set; }

    [JsonField("hoist")]
    public bool? Hoist { get; set; }

    [JsonField("permissions")]
    public ulong? Permissions { get; set; }

    [JsonField("mentionable")]
    public bool? IsMentionable { get; set; }

    [JsonField("icon_hash")]
    public string IconHash { get; set; }
}
