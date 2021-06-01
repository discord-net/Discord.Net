using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the privacy level.
    /// </summary>
    public enum PrivacyLevel
    {
        /// <summary>
        ///     The Stage instance is visible publicly, such as on Stage discovery.
        /// </summary>
        Public = 1,

        /// <summary>
        ///     The Stage instance is visible to only guild members.
        /// </summary>
        GuildOnly = 2,
    }
}
