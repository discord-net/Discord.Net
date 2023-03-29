using Discord.API.AuditLogs;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to an integration removal.
/// </summary>
public class IntegrationDeletedAuditLogData : IAuditLogData
{
    internal IntegrationDeletedAuditLogData(IntegrationInfo info)
    {
        Data = info;
    }

    internal static IntegrationDeletedAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<IntegrationInfoAuditLogModel>(changes, discord);
        
        return new(new IntegrationInfo(data));
    }

    /// <summary>
    ///     Gets the integration information before the changes.
    /// </summary>
    public IntegrationInfo Data { get; }
}
