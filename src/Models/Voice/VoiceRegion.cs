using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a voice region object.
    /// </summary>
    public record VoiceRegion
    {
        /// <summary>
        ///     Creates a <see cref="VoiceRegion"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Unique ID for the region.</param>
        /// <param name="name">Name of the region.</param>
        /// <param name="vip">True if this is a vip-only server.</param>
        /// <param name="optimal">True for a single server that is closest to the current user's client.</param>
        /// <param name="deprecated">Whether this is a deprecated voice region (avoid switching to these).</param>
        /// <param name="custom">Whether this is a custom voice region (used for events/etc).</param>
        [JsonConstructor]
        public VoiceRegion(string id, string name, bool vip, bool optimal, bool deprecated, bool custom)
        {
            Id = id;
            Name = name;
            Vip = vip;
            Optimal = optimal;
            Deprecated = deprecated;
            Custom = custom;
        }

        /// <summary>
        ///     Unique ID for the region.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        ///     Name of the region.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     True if this is a vip-only server.
        /// </summary>
        [JsonPropertyName("vip")]
        public bool Vip { get; }

        /// <summary>
        ///     True for a single server that is closest to the current user's client.
        /// </summary>
        [JsonPropertyName("optimal")]
        public bool Optimal { get; }

        /// <summary>
        ///     Whether this is a deprecated voice region (avoid switching to these).
        /// </summary>
        [JsonPropertyName("deprecated")]
        public bool Deprecated { get; }

        /// <summary>
        ///     Whether this is a custom voice region (used for events/etc).
        /// </summary>
        [JsonPropertyName("custom")]
        public bool Custom { get; }
    }
}
