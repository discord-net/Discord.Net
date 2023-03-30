using Discord.API.AuditLogs;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to an auto moderation rule removal.
/// </summary>
public class AutoModRuleDeletedAuditLogData : IAuditLogData
{
    private AutoModRuleDeletedAuditLogData(AutoModRuleInfo data)
    {
        Data = data;
    }

    internal static AutoModRuleDeletedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<AutoModRuleInfoAuditLogModel>(changes, discord);

        return new AutoModRuleDeletedAuditLogData(new (data));
    }

    /// <summary>
    ///     Gets the auto moderation rule information before the changes.
    /// </summary>
    public AutoModRuleInfo Data { get; }
}
