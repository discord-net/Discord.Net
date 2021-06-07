using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord presence object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/gateway#presence-update-presence-update-event-fields"/>
    /// </remarks>
    public record Presence
    {
        /// <summary>
        /// The <see cref="User"/> presence is being updated for.
        /// </summary>
        [JsonPropertyName("user")]
        public User? User { get; init; } // Required property candidate

        /// <summary>
        /// Id of the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; init; }

        /// <summary>
        /// The <see cref="User"/> status.
        /// </summary>
        [JsonPropertyName("status")]
        public UserStatus Status { get; init; }

        /// <summary>
        /// <see cref="User"/>'s current activities.
        /// </summary>
        [JsonPropertyName("activities")]
        public Activity[]? Activities { get; init; } // Required property candidate

        /// <summary>
        /// <see cref="User"/>'s platform-dependent status.
        /// </summary>
        [JsonPropertyName("client_status")]
        public ClientStatus? ClientStatus { get; init; } // Required property candidate
    }
}
