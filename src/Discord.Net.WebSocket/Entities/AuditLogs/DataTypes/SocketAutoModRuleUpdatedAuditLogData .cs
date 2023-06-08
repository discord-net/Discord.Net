using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an auto moderation rule update.
/// </summary>
public class AutoModRuleUpdatedAuditLogData : ISocketAuditLogData
{
    private AutoModRuleUpdatedAuditLogData(SocketAutoModRuleInfo before, SocketAutoModRuleInfo after)
    {
        Before = before;
        After = after;
    }

    internal static AutoModRuleUpdatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<AutoModRuleInfoAuditLogModel>(changes, discord);

        return new AutoModRuleUpdatedAuditLogData(new(before), new(after));
    }

    /// <summary>
    ///     Gets the auto moderation rule information before the changes.
    /// </summary>
    public SocketAutoModRuleInfo Before { get; }

    /// <summary>
    ///     Gets the auto moderation rule information after the changes.
    /// </summary>
    public SocketAutoModRuleInfo After { get; }
}
