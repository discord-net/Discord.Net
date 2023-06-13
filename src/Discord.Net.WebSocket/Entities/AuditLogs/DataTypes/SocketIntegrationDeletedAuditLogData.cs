using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an integration removal.
/// </summary>
public class SocketIntegrationDeletedAuditLogData : ISocketAuditLogData
{
    internal SocketIntegrationDeletedAuditLogData(SocketIntegrationInfo info)
    {
        Data = info;
    }

    internal static SocketIntegrationDeletedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<IntegrationInfoAuditLogModel>(changes, discord);
        
        return new(new SocketIntegrationInfo(data));
    }

    /// <summary>
    ///     Gets the integration information before the changes.
    /// </summary>
    public SocketIntegrationInfo Data { get; }
}
