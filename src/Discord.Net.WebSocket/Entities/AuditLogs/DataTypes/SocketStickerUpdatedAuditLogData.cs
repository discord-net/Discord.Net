using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a sticker update.
/// </summary>
public class SocketStickerUpdatedAuditLogData : ISocketAuditLogData
{
    internal SocketStickerUpdatedAuditLogData(ulong id, SocketStickerInfo before, SocketStickerInfo after)
    {
        StickerId = id;
        Before = before;
        After = after;
    }

    internal static SocketStickerUpdatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;
        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new SocketStickerUpdatedAuditLogData(entry.TargetId.Value, new(before), new (after));
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
    public SocketStickerInfo Before { get; }

    /// <summary>
    ///     Gets the sticker information after the changes.
    /// </summary>
    public SocketStickerInfo After { get; }
}
