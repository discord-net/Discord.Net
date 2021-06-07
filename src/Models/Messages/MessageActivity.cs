using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord message activity object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#message-object-message-activity-structure"/>
    /// </remarks>
    public record MessageActivity
    {
        /// <summary>
        /// Type of message activity.
        /// </summary>
        [JsonPropertyName("type")]
        public MessageActivityType Type { get; init; }

        /// <summary>
        /// <see cref="ActivityParty.Id"/> from a Rich Presence event.
        /// </summary>
        [JsonPropertyName("party_id")]
        public Optional<string> PartyId { get; init; }
    }
}
