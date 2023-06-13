using Discord.API.AuditLogs;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a webhook update.
/// </summary>
public class WebhookUpdateAuditLogData : IAuditLogData
{
    private WebhookUpdateAuditLogData(IWebhook webhook, WebhookInfo before, WebhookInfo after)
    {
        Webhook = webhook;
        Before = before;
        After = after;
    }

    internal static WebhookUpdateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<WebhookInfoAuditLogModel>(changes, discord);

        var webhookInfo = log.Webhooks?.FirstOrDefault(x => x.Id == entry.TargetId);
        var webhook = webhookInfo != null ? RestWebhook.Create(discord, (IGuild)null, webhookInfo) : null;

        return new WebhookUpdateAuditLogData(webhook, new(before), new(after));
    }

    /// <summary>
    ///     Gets the webhook that was updated.
    /// </summary>
    /// <returns>
    ///     A webhook object representing the webhook that was updated.
    /// </returns>
    public IWebhook Webhook { get; }

    /// <summary>
    ///     Gets the webhook information before the changes.
    /// </summary>
    /// <returns>
    ///     A webhook information object representing the webhook before the changes were made.
    /// </returns>
    public WebhookInfo Before { get; }

    /// <summary>
    ///     Gets the webhook information after the changes.
    /// </summary>
    /// <returns>
    ///     A webhook information object representing the webhook after the changes were made.
    /// </returns>
    public WebhookInfo After { get; }
}
