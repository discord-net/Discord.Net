using Discord.API.AuditLogs;
using System.Linq;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a webhook creation.
/// </summary>
public class WebhookCreateAuditLogData : IAuditLogData
{
    private WebhookCreateAuditLogData(IWebhook webhook, ulong webhookId, WebhookInfoAuditLogModel model)
    {
        Webhook = webhook;
        WebhookId = webhookId;
        Name = model.Name;
        Type = model.Type!.Value;
        ChannelId = model.ChannelId!.Value;
        Avatar = model.AvatarHash;
    }

    internal static WebhookCreateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<WebhookInfoAuditLogModel>(changes, discord);

        var webhookInfo = log.Webhooks?.FirstOrDefault(x => x.Id == entry.TargetId);
        var webhook = webhookInfo == null ? null : RestWebhook.Create(discord, (IGuild)null, webhookInfo);

        return new WebhookCreateAuditLogData(webhook, entry.TargetId!.Value, data);
    }

    // Doc Note: Corresponds to the *current* data

    /// <summary>
    ///     Gets the webhook that was created if it still exists.
    /// </summary>
    /// <returns>
    ///     A webhook object representing the webhook that was created if it still exists, otherwise returns <see langword="null" />.
    /// </returns>
    public IWebhook Webhook { get; }

    // Doc Note: Corresponds to the *audit log* data

    /// <summary>
    ///     Gets the webhook id.
    /// </summary>
    /// <returns>
    ///     The webhook identifier.
    /// </returns>
    public ulong WebhookId { get; }

    /// <summary>
    ///     Gets the type of webhook that was created.
    /// </summary>
    /// <returns>
    ///     The type of webhook that was created.
    /// </returns>
    public WebhookType Type { get; }

    /// <summary>
    ///     Gets the name of the webhook.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the webhook.
    /// </returns>
    public string Name { get; }
    /// <summary>
    ///     Gets the ID of the channel that the webhook could send to.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the channel that the webhook could send
    ///     to.
    /// </returns>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the hash value of the webhook's avatar.
    /// </summary>
    /// <returns>
    ///     A string containing the hash of the webhook's avatar.
    /// </returns>
    public string Avatar { get; }
}
