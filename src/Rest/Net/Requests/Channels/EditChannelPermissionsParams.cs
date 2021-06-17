using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record EditChannelPermissionsParams
    {
        /// <summary>
        /// The bitwise value of all allowed permissions.
        /// </summary>
        public Permissions Allow { get; set; }

        /// <summary>
        /// The bitwise value of all disallowed permissions.
        /// </summary>
        public Permissions Deny { get; set; }

        /// <summary>
        /// Type of overwrite.
        /// </summary>
        public OverwriteType Type { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
        }
    }
}
