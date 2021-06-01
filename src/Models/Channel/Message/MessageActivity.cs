using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a message activity object.
    /// </summary>
    public record MessageActivity
    {
        /// <summary>
        ///     Creates a <see cref="MessageActivity"/> with the provided parameters.
        /// </summary>
        /// <param name="type">Type of message activity.</param>
        /// <param name="partyId">Party_id from a Rich Presence event.</param>
        [JsonConstructor]
        public MessageActivity(int type, Optional<string> partyId)
        {
            Type = type;
            PartyId = partyId;
        }

        /// <summary>
        ///     Type of message activity.
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; }

        /// <summary>
        ///     Party_id from a Rich Presence event.
        /// </summary>
        [JsonPropertyName("party_id")]
        public Optional<string> PartyId { get; }
    }
}
