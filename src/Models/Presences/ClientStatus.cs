using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord client status object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/gateway#client-status-object"/>
    /// </remarks>
    public record ClientStatus
    {
        /// <summary>
        /// The <see cref="User"/>'s status set for an active
        /// desktop (Windows, Linux, Mac) application session.
        /// </summary>
        [JsonPropertyName("desktop")]
        public Optional<string> Desktop { get; init; }

        /// <summary>
        /// The <see cref="User"/>'s status set for an active mobile
        /// (iOS, Android) application session.
        /// </summary>
        [JsonPropertyName("mobile")]
        public Optional<string> Mobile { get; init; }

        /// <summary>
        /// The <see cref="User"/>'s status set for an active web
        /// (browser, bot account) application session.
        /// </summary>
        [JsonPropertyName("web")]
        public Optional<string> Web { get; init; }
    }
}
