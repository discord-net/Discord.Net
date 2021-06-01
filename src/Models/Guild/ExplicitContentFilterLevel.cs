using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the explicit content filter level.
    /// </summary>
    public enum ExplicitContentFilterLevel
    {
        /// <summary>
        ///     Media content will not be scanned.
        /// </summary>
        Disabled = 0,

        /// <summary>
        ///     Media content sent by members without roles will be scanned.
        /// </summary>
        MembersWithoutRoles = 1,

        /// <summary>
        ///     Media content sent by all members will be scanned.
        /// </summary>
        AllMembers = 2,
    }
}
