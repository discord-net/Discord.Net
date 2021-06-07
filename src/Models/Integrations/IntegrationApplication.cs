using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord integration application object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#integration-application-object-integration-application-structure"/>
    /// </remarks>
    public record IntegrationApplication
    {
        /// <summary>
        /// The id of the <see cref="IntegrationApplication"/>.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// The name of the <see cref="IntegrationApplication"/>.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// The icon hash of the <see cref="IntegrationApplication"/>.
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; init; }

        /// <summary>
        /// The description of the <see cref="IntegrationApplication"/>.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; init; } // Required property candidate

        /// <summary>
        /// The summary of the <see cref="IntegrationApplication"/>.
        /// </summary>
        [JsonPropertyName("summary")]
        public string? Summary { get; init; } // Required property candidate

        /// <summary>
        /// The <see cref="User"/> bot associated with this <see cref="IntegrationApplication"/>.
        /// </summary>
        [JsonPropertyName("bot")]
        public Optional<User> Bot { get; init; }
    }
}
