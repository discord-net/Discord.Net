using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an activity party object.
    /// </summary>
    public record ActivityParty
    {
        /// <summary>
        ///     Creates a <see cref="ActivityParty"/> with the provided parameters.
        /// </summary>
        /// <param name="id">The id of the party.</param>
        /// <param name="size">Used to show the party's current and maximum size.</param>
        [JsonConstructor]
        public ActivityParty(Optional<string> id, Optional<int[]> size)
        {
            Id = id;
            Size = size;
        }

        /// <summary>
        ///     The id of the party.
        /// </summary>
        [JsonPropertyName("id")]
        public Optional<string> Id { get; }

        /// <summary>
        ///     Used to show the party's current and maximum size.
        /// </summary>
        [JsonPropertyName("size")]
        public Optional<int[]> Size { get; }
    }
}
