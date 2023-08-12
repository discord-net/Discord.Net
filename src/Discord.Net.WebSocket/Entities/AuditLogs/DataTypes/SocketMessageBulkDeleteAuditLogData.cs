using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to message deletion(s).
/// </summary>
public class SocketMessageBulkDeleteAuditLogData : ISocketAuditLogData
{
    private SocketMessageBulkDeleteAuditLogData(ulong channelId, int count)
    {
        ChannelId = channelId;
        MessageCount = count;
    }

    internal static SocketMessageBulkDeleteAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        return new SocketMessageBulkDeleteAuditLogData(entry.TargetId!.Value, entry.Options.Count!.Value);
    }

    /// <summary>
    ///     Gets the ID of the channel that the messages were deleted from.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the messages were
    ///     deleted from.
    /// </returns>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the number of messages that were deleted.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the number of messages that were deleted from the channel.
    /// </returns>
    public int MessageCount { get; }
}
