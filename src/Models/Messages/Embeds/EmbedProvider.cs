using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord embed provider object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-provider-structure"/>
    /// </remarks>
    public record EmbedProvider
    {
        /// <summary>
        /// Name of provider.
        /// </summary>
        [JsonPropertyName("name")]
        public Optional<string> Name { get; init; }

        /// <summary>
        /// Url of provider.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; init; }
    }
}
