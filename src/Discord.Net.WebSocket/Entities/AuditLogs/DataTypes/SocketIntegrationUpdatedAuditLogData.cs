using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an integration update.
/// </summary>
public class SocketIntegrationUpdatedAuditLogData : ISocketAuditLogData
{
    internal SocketIntegrationUpdatedAuditLogData(SocketIntegrationInfo before, SocketIntegrationInfo after)
    {
        Before = before;
        After = after;
    }

    internal static SocketIntegrationUpdatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<IntegrationInfoAuditLogModel>(changes, discord);
        
        return new(new SocketIntegrationInfo(before), new SocketIntegrationInfo(after));
    }

    /// <summary>
    ///     Gets the integration information before the changes.
    /// </summary>
    public SocketIntegrationInfo Before { get; }

    /// <summary>
    ///     Gets the integration information after the changes.
    /// </summary>
    public SocketIntegrationInfo After { get; }
}
