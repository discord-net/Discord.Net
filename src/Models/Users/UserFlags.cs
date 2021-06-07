using System;

namespace Discord.Net.Models
{
    /// <summary>
    /// Declares a flag enum which represents the user flags for a <see cref="User"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/user#user-object-user-flags"/>
    /// </remarks>
    [Flags]
    public enum UserFlags
    {
        /// <summary>
        /// Default flag.
        /// </summary>
        None = 0,

        /// <summary>
        /// User is a Discord Employee.
        /// </summary>
        DiscordEmployee = 1 << 0,

        /// <summary>
        /// User is the owner of a partnered server.
        /// </summary>
        PartneredServerOwner = 1 << 1,

        /// <summary>
        /// User participated in HypeSquad events.
        /// </summary>
        HypeSquadEvents = 1 << 2,

        /// <summary>
        /// User is a level 1 Bug Hunter.
        /// </summary>
        BugHunterLevel1 = 1 << 3,

        /// <summary>
        /// User is part of House Bravery.
        /// </summary>
        HouseBravery = 1 << 6,

        /// <summary>
        ///  User is part of House Brilliance.
        /// </summary>
        HouseBrilliance = 1 << 7,

        /// <summary>
        ///  User is part of House Balance.
        /// </summary>
        HouseBalance = 1 << 8,

        /// <summary>
        /// User is an early supporter.
        /// </summary>
        EarlySupporter = 1 << 9,

        /// <summary>
        /// User is part of a team.
        /// </summary>
        TeamUser = 1 << 10,

        /// <summary>
        /// User is a level 2 Bug Hunter.
        /// </summary>
        BugHunterLevel2 = 1 << 14,

        /// <summary>
        /// User is a verified bot.
        /// </summary>
        VerifiedBot = 1 << 16,

        /// <summary>
        /// User is an early verified bot developer.
        /// </summary>
        EarlyVerifiedBotDeveloper = 1 << 17,

        /// <summary>
        /// User is a Discord certified moderator.
        /// </summary>
        DiscordCertifiedModerator = 1 << 18,
    }
}
