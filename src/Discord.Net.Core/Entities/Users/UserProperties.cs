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
        ///     Flag given to users who are a Discord employee.
        /// </summary>
        Staff = 1 << 0,
        /// <summary>
        ///     Flag given to users who are owners of a partnered Discord server.
        /// </summary>
        Partner = 1 << 1,
        /// <summary>
        ///     Flag given to users in HypeSquad events.
        /// </summary>
        HypeSquadEvents = 1 << 2,
        /// <summary>
        ///     Flag given to users who have participated in the bug report program.
        ///     This flag is obsolete, use <see cref="BugHunterLevel1"/> instead.
        /// </summary>
        [Obsolete("Use BugHunterLevel1 instead.")]
        BugHunter = 1 << 3,
        /// <summary>
        ///     Flag given to users who have participated in the bug report program and are level 1.
        /// </summary>
        BugHunterLevel1 = 1 << 3,
        /// <summary>
        ///     Flag given to users who are in the HypeSquad House of Bravery.
        /// </summary>
        HypeSquadBravery = 1 << 6,
        /// <summary>
        ///     Flag given to users who are in the HypeSquad House of Brilliance.
        /// </summary>
        HypeSquadBrilliance = 1 << 7,
        /// <summary>
        ///     Flag given to users who are in the HypeSquad House of Balance.
        /// </summary>
        HypeSquadBalance = 1 << 8,
        /// <summary>
        ///     Flag given to users who subscribed to Nitro before games were added.
        /// </summary>
        EarlySupporter = 1 << 9,
        /// <summary>
        ///     Flag given to users who are part of a team.
        /// </summary>
        TeamUser = 1 << 10,
        /// <summary>
        ///     Flag given to users who represent Discord (System).
        /// </summary>
        System = 1 << 12,
        /// <summary>
        ///     Flag given to users who have participated in the bug report program and are level 2.
        /// </summary>
        BugHunterLevel2 = 1 << 14,
        /// <summary>
        ///     Flag given to users who are verified bots.
        /// </summary>
        VerifiedBot = 1 << 16,
        /// <summary>
        ///     Flag given to users that developed bots and early verified their accounts.
        /// </summary>
        EarlyVerifiedBotDeveloper = 1 << 17,
    }
}
