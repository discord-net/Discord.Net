using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a sticker creation.
/// </summary>
public class SocketStickerCreatedAuditLogData : ISocketAuditLogData
{
    internal SocketStickerCreatedAuditLogData(ulong id, SocketStickerInfo data)
    {
        StickerId = id;
        Data = data;
    }

    internal static SocketStickerCreatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;
        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new SocketStickerCreatedAuditLogData(entry.TargetId.Value, new(data));
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
    public SocketStickerInfo Data { get; }
}
