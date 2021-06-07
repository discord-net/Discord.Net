namespace Discord.Net.Models
{
    /// <summary>
    /// Declares a flag enum which represents the activity type for an <see cref="Activity"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-types"/>
    /// </remarks>
    public enum ActivityType
    {
        /// <summary>
        /// Playing {name}.
        /// </summary>
        Game = 0,

        /// <summary>
        /// Streaming {details}.
        /// </summary>
        Streaming = 1,

        /// <summary>
        /// Listening to {name}.
        /// </summary>
        Listening = 2,

        /// <summary>
        /// Watching {name}.
        /// </summary>
        Watching = 3,

        /// <summary>
        /// {emoji} {name}.
        /// </summary>
        Custom = 4,

        /// <summary>
        /// Competing in {name}.
        /// </summary>
        Competing = 5,
    }
}
