using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a voice channel status delete.
/// </summary>
public class SocketVoiceChannelStatusDeleteAuditLogData : ISocketAuditLogData
{
    private SocketVoiceChannelStatusDeleteAuditLogData(ulong channelId)
    {
        ChannelId = channelId;
    }

    internal static SocketVoiceChannelStatusDeleteAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        return new (entry.TargetId!.Value);
    }

    /// <summary>
    ///     Get the id of the channel status was removed in.
    /// </summary>
    public ulong ChannelId { get; }
}
