using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the message activity type.
    /// </summary>
    [Flags]
    public enum MessageActivityType
    {
        /// <summary>
        ///     The message activity is to join.
        /// </summary>
        Join = 1,

        /// <summary>
        ///     The message activity is to spectate a stream.
        /// </summary>
        Spectate = 2,

        /// <summary>
        ///     The message activity is to listen to music.
        /// </summary>
        Listen = 3,

        /// <summary>
        ///     The message activity is to request to join.
        /// </summary>
        JoinRequest = 5,
    }
}
