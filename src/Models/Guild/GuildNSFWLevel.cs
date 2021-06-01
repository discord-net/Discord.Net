using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the guild NSFW level.
    /// </summary>
    public enum GuildNSFWLevel
    {
        /// <summary>
        ///     Default level, don't scan any content.
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Scan content from members without roles.
        /// </summary>
        Explicit = 1,

        /// <summary>
        ///     Scan content from all members.
        /// </summary>
        Safe = 2,

        /// <summary>
        ///     Server has an age restriction.
        /// </summary>
        AgeRestricted = 3,
    }
}
