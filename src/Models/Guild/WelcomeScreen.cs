using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a welcome screen object.
    /// </summary>
    public record WelcomeScreen
    {
        /// <summary>
        ///     Creates a <see cref="WelcomeScreen"/> with the provided parameters.
        /// </summary>
        /// <param name="description">The server description shown in the welcome screen.</param>
        /// <param name="welcomeChannels">The channels shown in the welcome screen, up to 5.</param>
        [JsonConstructor]
        public WelcomeScreen(string? description, WelcomeScreenChannel[] welcomeChannels)
        {
            Description = description;
            WelcomeChannels = welcomeChannels;
        }

        /// <summary>
        ///     The server description shown in the welcome screen.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; }

        /// <summary>
        ///     The channels shown in the welcome screen, up to 5.
        /// </summary>
        [JsonPropertyName("welcome_channels")]
        public WelcomeScreenChannel[] WelcomeChannels { get; }
    }
}
