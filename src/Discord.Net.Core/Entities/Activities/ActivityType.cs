namespace Discord
{
    /// <summary>
    ///     Specifies a Discord user's activity type.
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        ///     The user is playing a game.
        /// </summary>
        Playing = 0,
        /// <summary>
        ///     The user is streaming online.
        /// </summary>
        Streaming = 1,
        /// <summary>
        ///     The user is listening to a song.
        /// </summary>
        Listening = 2,
        /// <summary>
        ///     The user is watching some form of media.
        /// </summary>
        Watching = 3,
        /// <summary>
        ///     The user has set a custom status.
        /// </summary>
        CustomStatus = 4,
        /// <summary>
        ///     The user is competing in a game.
        /// </summary>
        Competing = 5,
    }
}
