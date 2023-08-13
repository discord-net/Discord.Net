namespace Discord;

/// <summary>
///     Represents a webhook object on Discord.
/// </summary>
public interface IWebhook : IDeletable, ISnowflakeEntity
{
    // TODO: different webhook types

    /// <summary>
    ///     Gets the token of this webhook.
    /// </summary>
    string Token { get; }

    /// <summary>
    ///     Gets the default name of this webhook.
    /// </summary>
    string? Name { get; }

    /// <summary>
    ///     Gets the ID of this webhook's default avatar.
    /// </summary>
    string? AvatarId { get; }

    /// <summary>
    ///     Gets the URL to this webhook's default avatar.
    /// </summary>
    string? GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128);

    /// <summary>
    ///     Gets the channel for this webhook.
    /// </summary>
    IEntitySource<IIntegrationChannel, ulong> Channel { get; }

    /// <summary>
    ///     Gets the guild owning this webhook.
    /// </summary>
    IEntitySource<IGuild, ulong>? Guild { get; }

    /// <summary>
    ///     Gets the user that created this webhook.
    /// </summary>
    IEntitySource<IUser, ulong> Creator { get; }

    /// <summary>
    ///     Gets the ID of the application owning this webhook.
    /// </summary>
    ulong? ApplicationId { get; }

    /// <summary>
    ///     Gets the type of this webhook.
    /// </summary>
    WebhookType Type { get; }

    /// <summary>
    ///     Modifies this webhook.
    /// </summary>
    Task ModifyAsync(Action<WebhookProperties> func, RequestOptions? options = null);
}
