using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a stage instance deleted.
/// </summary>
public class SocketStageInstanceDeleteAuditLogData : ISocketAuditLogData
{
    /// <summary>
    ///     Gets the topic of the stage channel.
    /// </summary>
    public string Topic { get; }

    /// <summary>
    ///     Gets the privacy level of the stage channel.
    /// </summary>
    public StagePrivacyLevel PrivacyLevel { get; }
    
    /// <summary>
    ///     Gets the Id of the stage channel.
    /// </summary>
    public ulong StageChannelId { get; }

    internal SocketStageInstanceDeleteAuditLogData(string topic, StagePrivacyLevel privacyLevel, ulong channelId)
    {
        Topic = topic;
        PrivacyLevel = privacyLevel;
        StageChannelId = channelId;
    }

    internal static SocketStageInstanceDeleteAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var topic = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "topic").OldValue.ToObject<string>(discord.ApiClient.Serializer);
        var privacyLevel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "privacy_level").OldValue.ToObject<StagePrivacyLevel>(discord.ApiClient.Serializer);
        var channelId = entry.Options.ChannelId;

        return new SocketStageInstanceDeleteAuditLogData(topic, privacyLevel, channelId ?? 0);
    }
}
