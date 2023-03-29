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
    
    internal SocketStageInfo(StagePrivacyLevel? level, string topic)
    {
        Topic = topic;
        PrivacyLevel = level;
    }
}
