namespace Discord.Rest
{
    /// <summary>
    ///     An information object representing a change in one of a guild member's roles.
    /// </summary>
    public struct MemberRoleEditInfo
    {
        internal MemberRoleEditInfo(string name, ulong roleId, bool added)
        {
            Name = name;
            RoleId = roleId;
            Added = added;
        }

        /// <summary>
        ///     Gets the name of the role that was changed.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the role that was changed.
        /// </returns>
        public string Name { get; }
        /// <summary>
        ///     Gets the ID of the role that was changed.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the role that was changed.
        /// </returns>
        public ulong RoleId { get; }
        /// <summary>
        ///     Gets a value that indicates whether the role was added to the user.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the role was added to the user; otherwise <c>false</c>.
        /// </returns>
        public bool Added { get; }
    }
}
