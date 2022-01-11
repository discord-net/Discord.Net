using System;

namespace Discord
{
    /// <summary>
    ///     Properties that are used to modify an <see cref="IRole" /> with the specified changes.
    /// </summary>
    /// <example>
    ///     The following example modifies the role to a mentionable one, renames the role into <c>Sonic</c>, and
    ///     changes the color to a light-blue.
    ///     <code language="cs">
    ///     await role.ModifyAsync(x =&gt;
    ///     {
    ///         x.Name = "Sonic";
    ///         x.Color = new Color(0x1A50BC);
    ///         x.Mentionable = true;
    ///     });
    ///     </code>
    /// </example>
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
        ///     Gets or sets the icon of the role.
        /// </summary>
        /// <remarks>
        ///     This value cannot be set at the same time as Emoji, as they are both exclusive.
        ///     
        ///     Setting an Icon will override a currently existing Emoji if present.
        /// </remarks>
        public Optional<Image?> Icon { get; set; }
        /// <summary>
        ///     Gets or sets the unicode emoji of the role.
        /// </summary>
        /// <remarks>
        ///     This value cannot be set at the same time as Icon, as they are both exclusive.
        ///
        ///     Setting an Emoji will override a currently existing Icon if present.
        /// </remarks>
        public Optional<Emoji> Emoji { get; set; }
        /// <summary>
        ///     Gets or sets whether or not this role can be mentioned.
        /// </summary>
        /// <remarks>
        ///     This value may not be set if the role is an @everyone role.
        /// </remarks>
        public Optional<bool> Mentionable { get; set; }
    }
}
