using Discord.API.AuditLogs;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a sticker update.
/// </summary>
public class StickerUpdatedAuditLogData : IAuditLogData
{
    internal StickerUpdatedAuditLogData(ulong id, StickerInfo before, StickerInfo after)
    {
        StickerId = id;
        Before = before;
        After = after;
    }

    internal static StickerUpdatedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        var changes = entry.Changes;
        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new StickerUpdatedAuditLogData(entry.TargetId.Value, new(before), new (after));
    }

    /// <summary>
    ///     Gets the snowflake ID of the updated sticker.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the updated sticker.
    /// </returns>
    public ulong StickerId { get; }

    /// <summary>
    ///     Gets the sticker information before the changes.
    /// </summary>
    public StickerInfo Before { get; }

    /// <summary>
    ///     Gets the sticker information after the changes.
    /// </summary>
    public StickerInfo After { get; }
}
