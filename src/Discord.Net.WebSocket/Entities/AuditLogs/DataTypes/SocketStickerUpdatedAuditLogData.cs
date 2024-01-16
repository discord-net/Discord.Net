using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a sticker update.
/// </summary>
public class SocketStickerUpdatedAuditLogData : ISocketAuditLogData
{
    internal SocketStickerUpdatedAuditLogData(SocketStickerInfo before, SocketStickerInfo after)
    {
        Before = before;
        After = after;
    }

    internal static SocketStickerUpdatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;
        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new SocketStickerUpdatedAuditLogData(new(before), new (after));
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
