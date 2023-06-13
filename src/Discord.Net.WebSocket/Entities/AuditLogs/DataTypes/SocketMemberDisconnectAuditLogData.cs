using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to disconnecting members from voice channels.
/// </summary>
public class SocketMemberDisconnectAuditLogData : ISocketAuditLogData
{
    private SocketMemberDisconnectAuditLogData(int count)
    {
        MemberCount = count;
    }

    internal static SocketMemberDisconnectAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        return new SocketMemberDisconnectAuditLogData(entry.Options.Count!.Value);
    }

    /// <summary>
    ///     Gets the number of members that were disconnected.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the number of members that were disconnected from a voice channel.
    /// </returns>
    public int MemberCount { get; }
}
