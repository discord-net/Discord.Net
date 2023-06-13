using Discord.Rest;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLogs.WebhookInfoAuditLogModel;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a webhook creation.
/// </summary>
public class SocketWebhookCreateAuditLogData : ISocketAuditLogData
{
    private SocketWebhookCreateAuditLogData(ulong webhookId, Model model)
    {
        WebhookId = webhookId;
        Name = model.Name;
        Type = model.Type!.Value;
        ChannelId = model.ChannelId!.Value;
        Avatar = model.AvatarHash;
    }

    internal static SocketWebhookCreateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<Model>(changes, discord);

        return new SocketWebhookCreateAuditLogData(entry.TargetId!.Value, data);
    }
    
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
