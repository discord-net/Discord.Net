namespace Discord.Rest
{
    /// <summary>
    ///     Represents information for a role edit.
    /// </summary>
    public struct RoleEditInfo
    {
        internal RoleEditInfo(Color? color, bool? mentionable, bool? hoist, string name,
            GuildPermissions? permissions)
        {
            Color = color;
            Mentionable = mentionable;
            Hoist = hoist;
            Name = name;
            Permissions = permissions;
        }

        /// <summary>
        ///     Gets the color of this role.
        /// </summary>
        /// <returns>
        ///     A color object representing the color assigned to this role; <c>null</c> if this role does not have a
        ///     color.
        /// </returns>
        public Color? Color { get; }
        /// <summary>
        ///     Gets a value that indicates whether this role is mentionable.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if other members can mention this role in a text channel; otherwise <c>false</c>; 
        ///     <c>null</c> if this is not mentioned in this entry.
        /// </returns>
        public bool? Mentionable { get; }
        /// <summary>
        ///     Gets a value that indicates whether this role is hoisted (i.e. its members will appear in a separate
        ///     section on the user list).
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this role's members will appear in a separate section in the user list; otherwise 
        ///     <c>false</c>; <c>null</c> if this is not mentioned in this entry.
        /// </returns>
        public bool? Hoist { get; }
        /// <summary>
        ///     Gets the name of this role.
        /// </summary>
        /// <returns>
        ///    A string containing the name of this role.
        /// </returns>
        public string Name { get; }
        /// <summary>
        ///     Gets the permissions assigned to this role.
        /// </summary>
        /// <returns>
        ///     A guild permissions object representing the permissions that have been assigned to this role; <c>null</c>
        ///     if no permissions have been assigned.
        /// </returns>
        public GuildPermissions? Permissions { get; }
    }
}
