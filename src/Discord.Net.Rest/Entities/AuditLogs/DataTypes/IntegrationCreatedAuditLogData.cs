using Discord.API.AuditLogs;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to an integration authorization.
/// </summary>
public class IntegrationCreatedAuditLogData : IAuditLogData
{
    internal IntegrationCreatedAuditLogData(IntegrationInfo info, IIntegration integration)
    {
        Integration = integration;
        Data = info;
    }

    internal static IntegrationCreatedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<IntegrationInfoAuditLogModel>(changes, discord);

        var integration = RestIntegration.Create(discord, null, log.Integrations.FirstOrDefault(x => x.Id == entry.TargetId));

        return new(new IntegrationInfo(data), integration);
    }

    /// <summary>
    ///     Gets the partial integration the changes correspond to.
    /// </summary>
    public IIntegration Integration { get; }

    /// <summary>
    ///     Gets the integration information after the changes.
    /// </summary>
    public IntegrationInfo Data { get; }
}
