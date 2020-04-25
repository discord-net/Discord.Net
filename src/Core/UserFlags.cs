using System;

namespace Discord.Core
{
    /// <summary>
    /// Flags which may be present on an <see cref="IUser"/>
    /// </summary>
    [Flags]
    public enum UserFlags
    {
        /// <summary>
        /// The user has no flags on their account.
        /// </summary>
        None = 0,
        /// <summary>
        /// The user is a Discord employee.
        /// </summary>
        Employee = 1 << 0,
        /// <summary>
        /// The user is a Discord partner.
        /// </summary>
        Parnter = 1 << 1,
        /// <summary>
        /// The user part of HypeSquad events.
        /// </summary>
        HypeSquad = 1 << 2,
        /// <summary>
        /// The user has the first level bug hunter badge.
        /// </summary>
        BugHunterLevel1 = 1 << 3,
        /// <summary>
        /// The user is part of HypeSquad Bravery.
        /// </summary>
        HypeSquadBravery = 1 << 6,
        /// <summary>
        /// The user is part of HypeSquad Brilliance.
        /// </summary>
        HypeSquadBrilliance = 1 << 7,
        /// <summary>
        /// The user is part of HypeSquad Balance.
        /// </summary>
        HypeSquadBalance = 1 << 8,
        /// <summary>
        /// The user is an early discord supporter.
        /// </summary>
        EarlySupporter = 1 << 9,
        /// <summary>
        /// The user is a team user.
        /// </summary>
        TeamUser = 1 << 10,
        /// <summary>
        /// The user is the Discord System user.
        /// </summary>
        System = 1 << 12,
        /// <summary>
        /// The user has the second level bug hunter badge.
        /// </summary>
        BugHunterLevel2 = 1 << 14,
        /// <summary>
        /// The user is a verified bot.
        /// </summary>
        VerifiedBot = 1 << 16,
        /// <summary>
        /// The user is a verified bot developer.
        /// </summary>
        VerifiedBotDeveloper = 1 << 17,
    }
}
