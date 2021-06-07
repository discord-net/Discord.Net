namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the explicit content filter level for a <see cref="Guild"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#guild-object-explicit-content-filter-level"/>
    /// </remarks>
    public enum ExplicitContentFilterLevel
    {
        /// <summary>
        /// Media content will not be scanned.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Media content sent by <see cref="GuildMember"/>s without
        /// <see cref="Role"/>s will be scanned.
        /// </summary>
        MembersWithoutRoles = 1,

        /// <summary>
        /// Media content sent by all <see cref="GuildMember"/>s will be scanned.
        /// </summary>
        AllMembers = 2,
    }
}
