using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an integration authorization.
/// </summary>
public class IntegrationCreatedAuditLogData : ISocketAuditLogData
{
    internal IntegrationCreatedAuditLogData(SocketIntegrationInfo info)
    {
        Data = info;
    }

    internal static IntegrationCreatedAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<IntegrationInfoAuditLogModel>(changes, discord);
        
        return new(new SocketIntegrationInfo(data));
    }

    /// <summary>
    ///     Gets the integration information after the changes.
    /// </summary>
    public SocketIntegrationInfo Data { get; }
}
