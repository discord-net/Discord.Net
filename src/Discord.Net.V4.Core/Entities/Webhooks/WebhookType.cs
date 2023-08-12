namespace Discord;

/// <summary>
///     Represents the type of a webhook.
/// </summary>
public enum WebhookType
{
    /// <summary>
    ///     An incoming webhook.
    /// </summary>
    Incoming = 1,

    /// <summary>
    ///     A webhook following a news channel.
    /// </summary>
    ChannelFollower = 2,

    /// <summary>
    ///     An application webhook used with interactions.
    /// </summary>
    Application = 3,
}
