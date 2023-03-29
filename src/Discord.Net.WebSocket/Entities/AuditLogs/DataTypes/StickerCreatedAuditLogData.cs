using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a sticker creation.
/// </summary>
public class StickerCreatedAuditLogData : ISocketAuditLogData
{
    internal StickerCreatedAuditLogData(SocketStickerInfo data)
    {
        Data = data;
    }

    internal static StickerCreatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;
        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<StickerInfoAuditLogModel>(changes, discord);

        return new StickerCreatedAuditLogData(new(data));
    }

    /// <summary>
    ///     Gets the sticker information after the changes.
    /// </summary>
    public SocketStickerInfo Data { get; }
}
