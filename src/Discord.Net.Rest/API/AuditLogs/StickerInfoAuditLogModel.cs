using Discord.Rest;

namespace Discord.API.AuditLogs;

internal class StickerInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("name")]
    public string Name { get; set; }

    [JsonField("tags")]
    public string Tags { get; set; }

    [JsonField("description")]
    public string Description { get; set; }
}
