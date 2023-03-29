using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a sticker update.
/// </summary>
public class StickerUpdatedAuditLogData : ISocketAuditLogData
{
    internal StickerUpdatedAuditLogData(SocketStickerInfo before, SocketStickerInfo after)
    {
        Before = before;
        After = after;
    }

    internal static StickerUpdatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;
        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new StickerUpdatedAuditLogData(new(before), new (after));
    }

    /// <summary>
    ///     Gets the sticker information before the changes.
    /// </summary>
    public SocketStickerInfo Before { get; }

    /// <summary>
    ///     Gets the sticker information after the changes.
    /// </summary>
    public SocketStickerInfo After { get; }
}
