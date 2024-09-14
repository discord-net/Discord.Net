using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a sticker removal.
/// </summary>
public class SocketStickerDeletedAuditLogData : ISocketAuditLogData
{
    internal SocketStickerDeletedAuditLogData(ulong id, SocketStickerInfo data)
    {
        StickerId = id;
        Data = data;
    }

    internal static SocketStickerDeletedAuditLogData Create(BaseDiscordClient discord, EntryModel entry)
    {
        var changes = entry.Changes;
        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new SocketStickerDeletedAuditLogData(entry.TargetId.Value, new(data));
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
    public SocketStickerInfo Data { get; }
}
