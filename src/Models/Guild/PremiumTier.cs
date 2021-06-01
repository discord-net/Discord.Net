using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the premium tier.
    /// </summary>
    public enum PremiumTier
    {
        /// <summary>
        ///     Guild has not unlocked any Server Boost perks.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Guild has unlocked Server Boost level 1 perks.
        /// </summary>
        Tier1 = 1,

        /// <summary>
        ///     Guild has unlocked Server Boost level 2 perks.
        /// </summary>
        Tier2 = 2,

        /// <summary>
        ///     Guild has unlocked Server Boost level 3 perks.
        /// </summary>
        Tier3 = 3,
    }
}
