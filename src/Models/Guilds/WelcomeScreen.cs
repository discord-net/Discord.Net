using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord welcome screen object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#welcome-screen-object-welcome-screen-structure"/>
    /// </remarks>
    public record WelcomeScreen
    {
        /// <summary>
        /// The <see cref="Guild.Description"/> shown in the <see cref="WelcomeScreen"/>.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; init; }

        /// <summary>
        /// The <see cref="WelcomeScreenChannel"/>s shown in the  <see cref="WelcomeScreen"/>, up to 5.
        /// </summary>
        [JsonPropertyName("welcome_channels")]
        public WelcomeScreenChannel[]? WelcomeChannels { get; init; } // Required property candidate
    }
}
