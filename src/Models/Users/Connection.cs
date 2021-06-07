using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord connection object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/user#connection-object-connection-structure"/>
    /// </remarks>
    public record Connection
    {
        /// <summary>
        /// Id of the <see cref="Connection"/> account.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; } // Required property candidate

        /// <summary>
        /// The username of the <see cref="Connection"/> account.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; } // Required property candidate

        /// <summary>
        /// The service of the <see cref="Connection"/> (twitch, youtube).
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; } // Required property candidate

        /// <summary>
        /// Whether the <see cref="Connection"/> is revoked.
        /// </summary>
        [JsonPropertyName("revoked")]
        public Optional<bool> Revoked { get; }

        /// <summary>
        /// An array of partial server <see cref="Integration"/>.
        /// </summary>
        [JsonPropertyName("integrations")]
        public Optional<Integration[]> Integrations { get; }

        /// <summary>
        /// Whether the <see cref="Connection"/> is verified.
        /// </summary>
        [JsonPropertyName("verified")]
        public bool Verified { get; }

        /// <summary>
        /// Whether friend sync is enabled for this <see cref="Connection"/>.
        /// </summary>
        [JsonPropertyName("friend_sync")]
        public bool FriendSync { get; }

        /// <summary>
        /// Whether activities related to this <see cref="Connection"/> will be shown in presence updates.
        /// </summary>
        [JsonPropertyName("show_activity")]
        public bool ShowActivity { get; }

        /// <summary>
        /// Visibility of this <see cref="Connection"/>.
        /// </summary>
        [JsonPropertyName("visibility")]
        public VisibilityType Visibility { get; }
    }
}
