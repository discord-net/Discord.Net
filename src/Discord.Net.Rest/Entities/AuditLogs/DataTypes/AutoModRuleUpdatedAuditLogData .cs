using Discord.API.AuditLogs;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to an auto moderation rule update.
/// </summary>
public class AutoModRuleUpdatedAuditLogData : IAuditLogData
{
    private AutoModRuleUpdatedAuditLogData(AutoModRuleInfo before, AutoModRuleInfo after, IAutoModRule rule)
    {
        Before = before;
        After = after;
        Rule = rule;
    }

    internal static AutoModRuleUpdatedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<AutoModRuleInfoAuditLogModel>(changes, discord);

        var rule = RestAutoModRule.Create(discord, log.AutoModerationRules.FirstOrDefault(x => x.Id == entry.TargetId));

        return new AutoModRuleUpdatedAuditLogData(new (before), new(after), rule);
    }

    /// <summary>
    ///     Gets the auto moderation rule the changes correspond to.
    /// </summary>
    public IAutoModRule Rule { get; }
    
    /// <summary>
    ///     Gets the auto moderation rule information before the changes.
    /// </summary>
    public AutoModRuleInfo Before { get; }

    /// <summary>
    ///     Gets the auto moderation rule information after the changes.
    /// </summary>
    public AutoModRuleInfo After { get; }
}
