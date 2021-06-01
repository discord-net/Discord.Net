namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the activity type.
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        ///     Playing {name}.
        /// </summary>
        Game = 0,

        /// <summary>
        ///     Streaming {details}.
        /// </summary>
        Streaming = 1,

        /// <summary>
        ///     Listening to {name}.
        /// </summary>
        Listening = 2,

        /// <summary>
        ///     Watching {name}.
        /// </summary>
        Watching = 3,

        /// <summary>
        ///     {emoji} {name}.
        /// </summary>
        Custom = 4,

        /// <summary>
        ///     Competing in {name}.
        /// </summary>
        Competing = 5,
    }
}
