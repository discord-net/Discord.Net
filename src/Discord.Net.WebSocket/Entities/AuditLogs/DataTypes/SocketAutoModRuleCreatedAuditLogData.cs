using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an auto moderation rule creation.
/// </summary>
public class SocketAutoModRuleCreatedAuditLogData : ISocketAuditLogData
{
    private SocketAutoModRuleCreatedAuditLogData(SocketAutoModRuleInfo data)
    {
        Data = data;
    }

    internal static SocketAutoModRuleCreatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<AutoModRuleInfoAuditLogModel>(changes, discord);

        return new SocketAutoModRuleCreatedAuditLogData(new (data));
    }

    /// <summary>
    ///     Gets the auto moderation rule information after the changes.
    /// </summary>
    public SocketAutoModRuleInfo Data { get; }
}
