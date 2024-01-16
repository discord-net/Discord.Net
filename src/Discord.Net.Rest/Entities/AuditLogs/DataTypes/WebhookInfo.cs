using Model = Discord.API.AuditLogs.WebhookInfoAuditLogModel;

namespace Discord.Rest;

/// <summary>
///     Represents information for a webhook.
/// </summary>
public struct WebhookInfo
{
    internal WebhookInfo(Model model)
    {
        Name = model.Name;
        ChannelId = model.ChannelId;
        Avatar = model.AvatarHash;
    }

    /// <summary>
    ///     Gets the name of this webhook.
    /// </summary>
    /// <returns>
    ///     A string containing the name of this webhook.
    /// </returns>
    public string Name { get; }

    /// <summary>
    ///     Gets the ID of the channel that this webhook sends to.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier of the channel that this webhook can send
    ///     to.
    /// </returns>
    public ulong? ChannelId { get; }

    /// <summary>
    ///     Gets the hash value of this webhook's avatar.
    /// </summary>
    /// <returns>
    ///     A string containing the hash of this webhook's avatar.
    /// </returns>
    public string Avatar { get; }
}
