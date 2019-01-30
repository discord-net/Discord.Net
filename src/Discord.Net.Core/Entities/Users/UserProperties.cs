using System;

namespace Discord
{
    [Flags]
    public enum UserProperties
    {
        /// <summary>
        ///     Default value for flags, when none are given to an account.
        /// </summary>
        None = 0,
        /// <summary>
        ///     Flag given to Discord staff.
        /// </summary>
        Staff = 0b1,
        /// <summary>
        ///     Flag given to Discord partners.
        /// </summary>
        Partner = 0b10,
        /// <summary>
        ///     Flag given to users who have participated in the bug report program.
        /// </summary>
        BugHunter = 0b1000,
        /// <summary>
        ///     Flag given to users who are in the HypeSquad House of Bravery.
        /// </summary>
        HypeSquadBravery = 0b100_0000,
        /// <summary>
        ///     Flag given to users who are in the HypeSquad House of Brilliance.
        /// </summary>
        HypeSquadBrilliance = 0b1000_0000,
        /// <summary>
        ///     Flag given to users who are in the HypeSquad House of Balance.
        /// </summary>
        HypeSquadBalance = 0b1_0000_0000,
        /// <summary>
        ///     Flag given to users who subscribed to Nitro before games were added.
        /// </summary>
        EarlySupporter = 0b10_0000_0000,
    }
}
