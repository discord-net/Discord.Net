using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the guild NSFW level.
    /// </summary>
    public enum GuildNsfwLevel
    {
        /// <summary>
        ///     Default level.
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Guild contains explicit content.
        /// </summary>
        Explicit = 1,

        /// <summary>
        ///     Guild is safe for work.
        /// </summary>
        Safe = 2,

        /// <summary>
        ///     Guild has an age restriction.
        /// </summary>
        AgeRestricted = 3,
    }
}
