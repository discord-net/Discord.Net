using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a presence object.
    /// </summary>
    public record Presence
    {
        /// <summary>
        ///     Creates a <see cref="Presence"/> with the provided parameters.
        /// </summary>
        /// <param name="user">The user presence is being updated for.</param>
        /// <param name="guildId">Id of the guild.</param>
        /// <param name="status">Either "idle", "dnd", "online", or "offline".</param>
        /// <param name="activities">User's current activities.</param>
        /// <param name="clientStatus">User's platform-dependent status.</param>
        [JsonConstructor]
        public Presence(User user, Snowflake guildId, string status, Activity[] activities, ClientStatus clientStatus)
        {
            User = user;
            GuildId = guildId;
            Status = status;
            Activities = activities;
            ClientStatus = clientStatus;
        }

        /// <summary>
        ///     The user presence is being updated for.
        /// </summary>
        [JsonPropertyName("user")]
        public User User { get; }

        /// <summary>
        ///     Id of the guild.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; }

        /// <summary>
        ///     Either "idle", "dnd", "online", or "offline".
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; }

        /// <summary>
        ///     User's current activities.
        /// </summary>
        [JsonPropertyName("activities")]
        public Activity[] Activities { get; }

        /// <summary>
        ///     User's platform-dependent status.
        /// </summary>
        [JsonPropertyName("client_status")]
        public ClientStatus ClientStatus { get; }
    }
}
