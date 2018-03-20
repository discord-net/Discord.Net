namespace Discord
{
    /// <summary>
    /// Defines user's activity type.
    /// </summary>
    public enum ActivityType
    {
        /// <summary> Activity that represents a user that is playing a game. </summary>
        Playing = 0,
        /// <summary> Activity that represents a user that is streaming online. </summary>
        Streaming = 1,
        /// <summary> Activity that represents a user that is listening to a song. </summary>
        Listening = 2,
        /// <summary> Activity that represents a user that is watching a media. </summary>
        Watching = 3
    }
}
