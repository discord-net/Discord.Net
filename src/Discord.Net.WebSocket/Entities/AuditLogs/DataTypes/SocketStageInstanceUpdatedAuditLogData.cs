using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a stage instance update.
/// </summary>
public class SocketStageInstanceUpdatedAuditLogData : ISocketAuditLogData
{
    /// <summary>
    ///     Gets the Id of the stage channel.
    /// </summary>
    public ulong StageChannelId { get; }

    /// <summary>
    ///     Gets the stage information before the changes.
    /// </summary>
    public SocketStageInfo Before { get; }

    /// <summary>
    ///     Gets the stage information after the changes.
    /// </summary>
    public SocketStageInfo After { get; }

    internal SocketStageInstanceUpdatedAuditLogData(ulong channelId, SocketStageInfo before, SocketStageInfo after)
    {
        StageChannelId = channelId;
        Before = before;
        After = after;
    }

    internal static SocketStageInstanceUpdatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var channelId = entry.Options.ChannelId.Value;

        var topic = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "topic");
        var privacy = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "privacy");
        
        var oldTopic = topic?.OldValue.ToObject<string>();
        var newTopic = topic?.NewValue.ToObject<string>();
        var oldPrivacy = privacy?.OldValue.ToObject<StagePrivacyLevel>();
        var newPrivacy = privacy?.NewValue.ToObject<StagePrivacyLevel>();

        return new SocketStageInstanceUpdatedAuditLogData(channelId, new SocketStageInfo(oldPrivacy, oldTopic), new SocketStageInfo(newPrivacy, newTopic));
    }
}
