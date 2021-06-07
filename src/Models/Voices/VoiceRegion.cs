using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord voice region object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/voice#voice-region-object-voice-region-structure"/>
    /// </remarks>
    public record VoiceRegion
    {
        /// <summary>
        /// Unique ID for the <see cref="VoiceRegion"/>.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; init; } // Required property candidate

        /// <summary>
        /// Name of the <see cref="VoiceRegion"/>.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// True if this is a vip-only server.
        /// </summary>
        [JsonPropertyName("vip")]
        public bool Vip { get; init; }

        /// <summary>
        /// True for a single server that is closest to the current user's client.
        /// </summary>
        [JsonPropertyName("optimal")]
        public bool Optimal { get; init; }

        /// <summary>
        /// Whether this is a deprecated <see cref="VoiceRegion"/> (avoid switching to these).
        /// </summary>
        [JsonPropertyName("deprecated")]
        public bool Deprecated { get; init; }

        /// <summary>
        /// Whether this is a custom <see cref="VoiceRegion"/> (used for events, etc).
        /// </summary>
        [JsonPropertyName("custom")]
        public bool Custom { get; init; }
    }
}
