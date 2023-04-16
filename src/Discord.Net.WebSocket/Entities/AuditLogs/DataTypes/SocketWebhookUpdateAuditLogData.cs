using Discord.Rest;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLogs.WebhookInfoAuditLogModel;


namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a webhook update.
/// </summary>
public class SocketWebhookUpdateAuditLogData : ISocketAuditLogData
{
    private SocketWebhookUpdateAuditLogData(SocketWebhookInfo before, SocketWebhookInfo after)
    {
        Before = before;
        After = after;
    }

    internal static SocketWebhookUpdateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<Model>(changes, discord);

        return new SocketWebhookUpdateAuditLogData(new(before), new(after));
    }

    /// <summary>
    ///     Gets the webhook information before the changes.
    /// </summary>
    /// <returns>
    ///     A webhook information object representing the webhook before the changes were made.
    /// </returns>
    public SocketWebhookInfo Before { get; }

    /// <summary>
    ///     Gets the webhook information after the changes.
    /// </summary>
    /// <returns>
    ///     A webhook information object representing the webhook after the changes were made.
    /// </returns>
    public SocketWebhookInfo After { get; }
}
