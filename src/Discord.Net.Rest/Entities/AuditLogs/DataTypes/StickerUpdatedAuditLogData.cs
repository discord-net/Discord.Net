using Discord.API.AuditLogs;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a sticker update.
/// </summary>
public class StickerUpdatedAuditLogData : IAuditLogData
{
    internal StickerUpdatedAuditLogData(StickerInfo before, StickerInfo after)
    {
        Before = before;
        After = after;
    }

    internal static StickerUpdatedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        var changes = entry.Changes;
        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new StickerUpdatedAuditLogData(new(before), new (after));
    }

    /// <summary>
    ///     Gets the sticker information before the changes.
    /// </summary>
    public StickerInfo Before { get; }

    /// <summary>
    ///     Gets the sticker information after the changes.
    /// </summary>
    public StickerInfo After { get; }
}
