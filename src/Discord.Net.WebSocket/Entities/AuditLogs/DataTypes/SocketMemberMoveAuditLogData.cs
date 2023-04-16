using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to moving members between voice channels.
/// </summary>
public class SocketMemberMoveAuditLogData : ISocketAuditLogData
{
    private SocketMemberMoveAuditLogData(ulong channelId, int count)
    {
        ChannelId = channelId;
        MemberCount = count;
    }

    internal static SocketMemberMoveAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        return new SocketMemberMoveAuditLogData(entry.Options.ChannelId!.Value, entry.Options.Count!.Value);
    }

    /// <summary>
    ///     Gets the ID of the channel that the members were moved to.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the members were moved to.
    /// </returns>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the number of members that were moved.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the number of members that were moved to another voice channel.
    /// </returns>
    public int MemberCount { get; }
}
