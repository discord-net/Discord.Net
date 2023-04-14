using Discord.API.AuditLogs;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to an auto moderation rule creation.
/// </summary>
public class AutoModRuleCreatedAuditLogData : IAuditLogData
{
    private AutoModRuleCreatedAuditLogData(AutoModRuleInfo data)
    {
        Data = data;
    }

    internal static AutoModRuleCreatedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<AutoModRuleInfoAuditLogModel>(changes, discord);

        return new AutoModRuleCreatedAuditLogData(new (data));
    }

    /// <summary>
    ///     Gets the auto moderation rule information after the changes.
    /// </summary>
    public AutoModRuleInfo Data { get; }
}
