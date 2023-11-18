using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a voice channel status delete.
/// </summary>
public class VoiceChannelStatusDeletedAuditLogData : IAuditLogData
{
    private VoiceChannelStatusDeletedAuditLogData(ulong channelId)
    {
        ChannelId = channelId;
    }

    internal static VoiceChannelStatusDeletedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        return new (entry.TargetId!.Value);
    }

    /// <summary>
    ///     Get the id of the channel status was removed in.
    /// </summary>
    public ulong ChannelId { get; }
}
