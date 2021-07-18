using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyGuildWelcomeScreenParams
    {
        /// <summary>
        /// Whether the welcome screen is enabled.
        /// </summary>
        public Optional<bool?> Enabled { get; set; }

        /// <summary>
        /// Channels linked in the welcome screen and their display options.
        /// </summary>
        public Optional<WelcomeScreenChannel[]?> WelcomeChannels { get; set; }

        /// <summary>
        /// The server description to show in the welcome screen.
        /// </summary>
        public Optional<string?> Description { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
        }
    }
}
