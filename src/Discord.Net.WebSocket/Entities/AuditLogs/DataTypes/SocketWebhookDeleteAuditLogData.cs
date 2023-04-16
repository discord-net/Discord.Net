using Discord.Rest;

using Model = Discord.API.AuditLogs.WebhookInfoAuditLogModel;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a webhook deletion.
/// </summary>
public class SocketWebhookDeleteAuditLogData : ISocketAuditLogData
{
    private SocketWebhookDeleteAuditLogData(ulong id, Model model)
    {
        WebhookId = id;
        ChannelId = model.ChannelId!.Value;
        Name = model.Name;
        Type = model.Type!.Value;
        Avatar = model.AvatarHash;
    }

    internal static SocketWebhookDeleteAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<Model>(changes, discord);

        return new SocketWebhookDeleteAuditLogData(entry.TargetId!.Value,data);
    }

    /// <summary>
    ///     Gets the ID of the webhook that was deleted.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the webhook that was deleted.
    /// </returns>
    public ulong WebhookId { get; }

    /// <summary>
    ///     Gets the ID of the channel that the webhook could send to.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the channel that the webhook could send
    ///     to.
    /// </returns>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the type of the webhook that was deleted.
    /// </summary>
    /// <returns>
    ///     The type of webhook that was deleted.
    /// </returns>
    public WebhookType Type { get; }

    /// <summary>
    ///     Gets the name of the webhook that was deleted.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the webhook that was deleted.
    /// </returns>
    public string Name { get; }

    /// <summary>
    ///     Gets the hash value of the webhook's avatar.
    /// </summary>
    /// <returns>
    ///     A string containing the hash of the webhook's avatar.
    /// </returns>
    public string Avatar { get; }
}
