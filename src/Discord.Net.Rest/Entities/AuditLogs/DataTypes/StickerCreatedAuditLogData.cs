using Discord.API.AuditLogs;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a sticker creation.
/// </summary>
public class StickerCreatedAuditLogData : IAuditLogData
{
    internal StickerCreatedAuditLogData(ulong id, StickerInfo data)
    {
        StickerId = id;
        Data = data;
    }

    internal static StickerCreatedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        var changes = entry.Changes;
        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new StickerCreatedAuditLogData(entry.TargetId.Value, new(data));
    }

    /// <summary>
    ///     Gets the snowflake ID of the created sticker.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the created sticker.
    /// </returns>
    public ulong StickerId { get; }

    /// <summary>
    ///     Gets the sticker information after the changes.
    /// </summary>
    public StickerInfo Data { get; }
}
