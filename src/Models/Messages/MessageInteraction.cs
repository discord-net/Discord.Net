using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord message interaction object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/interactions/slash-commands#messageinteraction"/>
    /// </remarks>
    public record MessageInteraction
    {
        /// <summary>
        /// Id of the interaction.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// The type of interaction.
        /// </summary>
        [JsonPropertyName("type")]
        public InteractionType Type { get; init; }

        /// <summary>
        /// The name of the ApplicationCommand.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// The <see cref="Models.User"/> who invoked the interaction.
        /// </summary>
        [JsonPropertyName("user")]
        public User? User { get; init; } // Required property candidate
    }
}
