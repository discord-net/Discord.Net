using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord activity party object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-party"/>
    /// </remarks>
    public record ActivityParty
    {
        /// <summary>
        /// The id of the party.
        /// </summary>
        [JsonPropertyName("id")]
        public Optional<string> Id { get; init; }

        /// <summary>
        /// Used to show the party's current and maximum size.
        /// </summary>
        /// <remarks>
        /// Array of two integers (current_size, max_size).
        /// </remarks>
        [JsonPropertyName("size")]
        public Optional<int[]> Size { get; init; }
    }
}
