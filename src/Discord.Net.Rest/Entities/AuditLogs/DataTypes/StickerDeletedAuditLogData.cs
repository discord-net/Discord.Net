using Discord.API.AuditLogs;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a sticker removal.
/// </summary>
public class StickerDeletedAuditLogData : IAuditLogData
{
    internal StickerDeletedAuditLogData(ulong id, StickerInfo data)
    {
        StickerId = id;
        Data = data;
    }

    internal static StickerDeletedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        var changes = entry.Changes;
        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new StickerDeletedAuditLogData(entry.TargetId.Value, new(data));
    }

    /// <summary>
    ///     Gets the snowflake ID of the deleted sticker.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the deleted sticker.
    /// </returns>
    public ulong StickerId { get; }

    /// <summary>
    ///     Gets the sticker information before the changes.
    /// </summary>
    public StickerInfo Data { get; }
}
