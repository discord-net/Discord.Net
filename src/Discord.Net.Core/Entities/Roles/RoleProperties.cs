namespace Discord
{
    /// <summary>
    /// Modify an IRole with the specified parameters
    /// </summary>
    /// <example>
    /// <code language="c#">
    /// await role.ModifyAsync(x =>
    /// {
    ///     x.Color = new Color(180, 15, 40);
    ///     x.Hoist = true;
    /// });
    /// </code>
    /// </example>
    /// <seealso cref="IRole"/>
    public class RoleProperties
    {
        /// <summary>
        /// The name of the role
        /// </summary>
        /// <remarks>
        /// If this role is the EveryoneRole, this value may not be set.
        /// </remarks>
        public Optional<string> Name { get; set; }
        /// <summary>
        /// The role's GuildPermissions
        /// </summary>
        public Optional<GuildPermissions> Permissions { get; set; }
        /// <summary>
        /// The position of the role. This is 0-based!
        /// </summary>
        /// <remarks>
        /// If this role is the EveryoneRole, this value may not be set.
        /// </remarks>
        public Optional<int> Position { get; set; }
        /// <summary>
        /// The color of the Role.
        /// </summary>
        /// <remarks>
        /// If this role is the EveryoneRole, this value may not be set.
        /// </remarks>
        public Optional<Color> Color { get; set; }
        /// <summary>
        /// Whether or not this role should be displayed independently in the userlist.
        /// </summary>
        /// <remarks>
        /// If this role is the EveryoneRole, this value may not be set.
        /// </remarks>
        public Optional<bool> Hoist { get; set; }
        /// <summary>
        /// Whether or not this role can be mentioned.
        /// </summary>
        /// <remarks>
        /// If this role is the EveryoneRole, this value may not be set.
        /// </remarks>
        public Optional<bool> Mentionable { get; set; }
    }
}
