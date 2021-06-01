using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a client status object.
    /// </summary>
    public record ClientStatus
    {
        /// <summary>
        ///     Creates a <see cref="ClientStatus"/> with the provided parameters.
        /// </summary>
        /// <param name="desktop">The user's status set for an active desktop (Windows, Linux, Mac) application session.</param>
        /// <param name="mobile">The user's status set for an active mobile (iOS, Android) application session.</param>
        /// <param name="web">The user's status set for an active web (browser, bot account) application session.</param>
        [JsonConstructor]
        public ClientStatus(Optional<string> desktop, Optional<string> mobile, Optional<string> web)
        {
            Desktop = desktop;
            Mobile = mobile;
            Web = web;
        }

        /// <summary>
        ///     The user's status set for an active desktop (Windows, Linux, Mac) application session.
        /// </summary>
        [JsonPropertyName("desktop")]
        public Optional<string> Desktop { get; }

        /// <summary>
        ///     The user's status set for an active mobile (iOS, Android) application session.
        /// </summary>
        [JsonPropertyName("mobile")]
        public Optional<string> Mobile { get; }

        /// <summary>
        ///     The user's status set for an active web (browser, bot account) application session.
        /// </summary>
        [JsonPropertyName("web")]
        public Optional<string> Web { get; }
    }
}
