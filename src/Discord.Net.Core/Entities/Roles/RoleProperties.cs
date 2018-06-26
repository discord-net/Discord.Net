namespace Discord
{
    /// <summary>
    ///     Properties that are used to modify an <see cref="IRole" /> with the specified changes.
    /// </summary>
    /// <seealso cref="IRole.ModifyAsync" />
    public class RoleProperties
    {
        /// <summary>
        ///     Gets or sets the name of the role.
        /// </summary>
        /// <remarks>
        ///     This value may not be set if the role is an @everyone role.
        /// </remarks>
        public Optional<string> Name { get; set; }
        /// <summary>
        ///     Gets or sets the role's <see cref="GuildPermission"/>.
        /// </summary>
        public Optional<GuildPermissions> Permissions { get; set; }
        /// <summary>
        ///     Gets or sets the position of the role. This is 0-based!
        /// </summary>
        /// <remarks>
        ///     This value may not be set if the role is an @everyone role.
        /// </remarks>
        public Optional<int> Position { get; set; }
        /// <summary>
        ///     Gets or sets the color of the role.
        /// </summary>
        /// <remarks>
        ///     This value may not be set if the role is an @everyone role.
        /// </remarks>
        public Optional<Color> Color { get; set; }
        /// <summary>
        ///     Gets or sets whether or not this role should be displayed independently in the user list.
        /// </summary>
        /// <remarks>
        ///     This value may not be set if the role is an @everyone role.
        /// </remarks>
        public Optional<bool> Hoist { get; set; }
        /// <summary>
        ///     Gets or sets whether or not this role can be mentioned.
        /// </summary>
        /// <remarks>
        ///     This value may not be set if the role is an @everyone role.
        /// </remarks>
        public Optional<bool> Mentionable { get; set; }
    }
}
