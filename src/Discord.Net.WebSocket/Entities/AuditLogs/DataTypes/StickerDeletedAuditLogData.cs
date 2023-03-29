using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a sticker removal.
/// </summary>
public class StickerDeletedAuditLogData : ISocketAuditLogData
{
    internal StickerDeletedAuditLogData(SocketStickerInfo data)
    {
        Data = data;
    }

    internal static StickerDeletedAuditLogData Create(BaseDiscordClient discord, EntryModel entry)
    {
        var changes = entry.Changes;
        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new StickerDeletedAuditLogData(new(data));
    }

    /// <summary>
    ///     Gets the sticker information before the changes.
    /// </summary>
    public SocketStickerInfo Data { get; }
}
