namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the message activity type for a <see cref="Message"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#message-object-message-activity-types"/>
    /// </remarks>
    public enum MessageActivityType
    {
        /// <summary>
        /// The message activity is to join.
        /// </summary>
        Join = 1,

        /// <summary>
        /// The message activity is to spectate a stream.
        /// </summary>
        Spectate = 2,

        /// <summary>
        /// The message activity is to listen to music.
        /// </summary>
        Listen = 3,

        /// <summary>
        /// The message activity is to request to join.
        /// </summary>
        JoinRequest = 5,
    }
}
