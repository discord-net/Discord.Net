using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord activity secrets object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-secrets"/>
    /// </remarks>
    public record ActivitySecrets
    {
        /// <summary>
        /// The secret for joining a party.
        /// </summary>
        [JsonPropertyName("join")]
        public Optional<string> Join { get; init; }

        /// <summary>
        /// The secret for spectating a game.
        /// </summary>
        [JsonPropertyName("spectate")]
        public Optional<string> Spectate { get; init; }

        /// <summary>
        /// The secret for a specific instanced match.
        /// </summary>
        [JsonPropertyName("match")]
        public Optional<string> Match { get; init; }
    }
}
