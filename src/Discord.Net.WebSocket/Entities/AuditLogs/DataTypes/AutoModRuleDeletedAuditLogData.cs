using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an auto moderation rule removal.
/// </summary>
public class AutoModRuleDeletedAuditLogData : ISocketAuditLogData
{
    private AutoModRuleDeletedAuditLogData(SocketAutoModRuleInfo data)
    {
        Data = data;
    }

    internal static AutoModRuleDeletedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<AutoModRuleInfoAuditLogModel>(changes, discord);

        return new AutoModRuleDeletedAuditLogData(new (data));
    }

    /// <summary>
    ///     Gets the auto moderation rule information before the changes.
    /// </summary>
    public SocketAutoModRuleInfo Data { get; }
}
