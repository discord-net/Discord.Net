using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateGuildRoleParams
    {
        /// <summary>
        /// Name of the role.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        /// Bitwise value of the enabled/disabled permissions.
        /// </summary>
        public Optional<Permissions> Permissions { get; set; }

        /// <summary>
        /// Role color.
        /// </summary>
        public Optional<Color> Color { get; set; }

        /// <summary>
        /// Whether the role should be displayed separately in the sidebar.
        /// </summary>
        public Optional<bool> Hoist { get; set; }

        /// <summary>
        /// Whether the role should be mentionable.
        /// </summary>
        public Optional<bool> Mentionable { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNullOrEmpty(Name!, nameof(Name));
        }
    }
}
