using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord activity object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-structure"/>
    /// </remarks>
    public record Activity
    {
        /// <summary>
        /// The <see cref="Activity"/>'s name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// Activity type.
        /// </summary>
        [JsonPropertyName("type")]
        public ActivityType Type { get; init; }

        /// <summary>
        /// Stream url, is validated when <see cref="Type"/> is <see cref="ActivityType.Streaming"/>.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string?> Url { get; init; }

        /// <summary>
        /// Unix timestamp of when the <see cref="Activity"/> was added to the <see cref="User"/>'s session.
        /// </summary>
        [JsonPropertyName("created_at")]
        public int CreatedAt { get; init; }

        /// <summary>
        /// Unix timestamps for start and/or end of the game.
        /// </summary>
        [JsonPropertyName("timestamps")]
        public Optional<ActivityTimestamps> Timestamps { get; init; }

        /// <summary>
        /// <see cref="Application"/> id for the game.
        /// </summary>
        [JsonPropertyName("application_id")]
        public Optional<Snowflake> ApplicationId { get; init; }

        /// <summary>
        /// What the player is currently doing.
        /// </summary>
        [JsonPropertyName("details")]
        public Optional<string?> Details { get; init; }

        /// <summary>
        /// The <see cref="User"/>'s current party status.
        /// </summary>
        [JsonPropertyName("state")]
        public Optional<string?> State { get; init; }

        /// <summary>
        /// The <see cref="Models.Emoji"/> used for a custom status.
        /// </summary>
        [JsonPropertyName("emoji")]
        public Optional<Emoji?> Emoji { get; init; }

        /// <summary>
        /// Information for the current party of the player.
        /// </summary>
        [JsonPropertyName("party")]
        public Optional<ActivityParty> Party { get; init; }

        /// <summary>
        /// Images for the presence and their hover texts.
        /// </summary>
        [JsonPropertyName("assets")]
        public Optional<ActivityAssets> Assets { get; init; }

        /// <summary>
        /// Secrets for Rich Presence joining and spectating.
        /// </summary>
        [JsonPropertyName("secrets")]
        public Optional<ActivitySecrets> Secrets { get; init; }

        /// <summary>
        /// Whether or not the <see cref="Activity"/> is an instanced game session.
        /// </summary>
        [JsonPropertyName("instance")]
        public Optional<bool> Instance { get; init; }

        /// <summary>
        /// Activity flags ORd together, describes what the payload includes.
        /// </summary>
        [JsonPropertyName("flags")]
        public Optional<ActivityFlags> Flags { get; init; }

        /// <summary>
        /// The custom buttons shown in the Rich Presence (max 2).
        /// </summary>
        [JsonPropertyName("buttons")]
        public Optional<string[]> ButtonLabels { get; init; }
    }
}
