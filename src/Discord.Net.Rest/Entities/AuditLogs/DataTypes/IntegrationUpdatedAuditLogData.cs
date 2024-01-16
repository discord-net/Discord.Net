using Discord.API.AuditLogs;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to an integration update.
/// </summary>
public class IntegrationUpdatedAuditLogData : IAuditLogData
{
    internal IntegrationUpdatedAuditLogData(IntegrationInfo before, IntegrationInfo after, IIntegration integration)
    {
        Before = before;
        After = after;
        Integration = integration;
    }

    internal static IntegrationUpdatedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<IntegrationInfoAuditLogModel>(changes, discord);

        var integration = RestIntegration.Create(discord, null, log.Integrations.FirstOrDefault(x => x.Id == entry.TargetId));

        return new(new IntegrationInfo(before), new IntegrationInfo(after), integration);
    }

    /// <summary>
    ///     Gets the partial integration the changes correspond to.
    /// </summary>
    public IIntegration Integration { get; }

    /// <summary>
    ///     Gets the integration information before the changes.
    /// </summary>
    public IntegrationInfo Before { get; }

    /// <summary>
    ///     Gets the integration information after the changes.
    /// </summary>
    public IntegrationInfo After { get; }
}
