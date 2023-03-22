namespace Discord.WebSocket;

/// <summary>
///     Represents information for a stage.
/// </summary>
public class SocketStageInfo
{
    /// <summary>
    ///     Gets the topic of the stage channel.
    /// </summary>
    public string Topic { get; }

    /// <summary>
    ///     Gets the privacy level of the stage channel.
    /// </summary>
    public StagePrivacyLevel? PrivacyLevel { get; }

    /// <summary>
    ///     Gets the user who started the stage channel.
    /// </summary>
    public IUser User { get; }

    internal SocketStageInfo(IUser user, StagePrivacyLevel? level, string topic)
    {
        Topic = topic;
        PrivacyLevel = level;
        User = user;
    }
}
