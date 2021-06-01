using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a embed provider object.
    /// </summary>
    public record EmbedProvider
    {
        /// <summary>
        ///     Creates a <see cref="EmbedProvider"/> with the provided parameters.
        /// </summary>
        /// <param name="name">Name of provider.</param>
        /// <param name="url">Url of provider.</param>
        [JsonConstructor]
        public EmbedProvider(Optional<string> name, Optional<string> url)
        {
            Name = name;
            Url = url;
        }

        /// <summary>
        ///     Name of provider.
        /// </summary>
        [JsonPropertyName("name")]
        public Optional<string> Name { get; }

        /// <summary>
        ///     Url of provider.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; }
    }
}
