namespace Discord;

/// <summary>
///     Represents the type of a webhook.
/// </summary>
/// <remarks>
///     This type is currently unused, and is only returned in audit log responses.
/// </remarks>
public enum WebhookType
{
    /// <summary>
    ///     An incoming webhook.
    /// </summary>
    Incoming = 1,

    /// <summary>
    ///     A channel follower webhook.
    /// </summary>
    ChannelFollower = 2,

    /// <summary>
    ///     An application (interaction) webhook.
    /// </summary>
    Application = 3,
}
