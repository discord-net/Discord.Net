using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an activity object.
    /// </summary>
    public record Activity
    {
        /// <summary>
        ///     Creates a <see cref="Activity"/> with the provided parameters.
        /// </summary>
        /// <param name="name">The activity's name.</param>
        /// <param name="type">Activity type.</param>
        /// <param name="url">Stream url, is validated when type is 1.</param>
        /// <param name="createdAt">Unix timestamp of when the activity was added to the user's session.</param>
        /// <param name="timestamps">Unix timestamps for start and/or end of the game.</param>
        /// <param name="applicationId">Application id for the game.</param>
        /// <param name="details">What the player is currently doing.</param>
        /// <param name="state">The user's current party status.</param>
        /// <param name="emoji">The emoji used for a custom status.</param>
        /// <param name="party">Information for the current party of the player.</param>
        /// <param name="assets">Images for the presence and their hover texts.</param>
        /// <param name="secrets">Secrets for Rich Presence joining and spectating.</param>
        /// <param name="instance">Whether or not the activity is an instanced game session.</param>
        /// <param name="flags">Activity flags ORd together, describes what the payload includes.</param>
        /// <param name="buttonLabels">The custom buttons shown in the Rich Presence (max 2).</param>
        [JsonConstructor]
        public Activity(string name, int type, Optional<string?> url, int createdAt, Optional<ActivityTimestamps> timestamps, Optional<Snowflake> applicationId, Optional<string?> details, Optional<string?> state, Optional<Emoji?> emoji, Optional<ActivityParty> party, Optional<ActivityAssets> assets, Optional<ActivitySecrets> secrets, Optional<bool> instance, Optional<int> flags, Optional<string[]> buttonLabels)
        {
            Name = name;
            Type = type;
            Url = url;
            CreatedAt = createdAt;
            Timestamps = timestamps;
            ApplicationId = applicationId;
            Details = details;
            State = state;
            Emoji = emoji;
            Party = party;
            Assets = assets;
            Secrets = secrets;
            Instance = instance;
            Flags = flags;
            ButtonLabels = buttonLabels;
        }

        /// <summary>
        ///     The activity's name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     Activity type.
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; }

        /// <summary>
        ///     Stream url, is validated when type is 1.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string?> Url { get; }

        /// <summary>
        ///     Unix timestamp of when the activity was added to the user's session.
        /// </summary>
        [JsonPropertyName("created_at")]
        public int CreatedAt { get; }

        /// <summary>
        ///     Unix timestamps for start and/or end of the game.
        /// </summary>
        [JsonPropertyName("timestamps")]
        public Optional<ActivityTimestamps> Timestamps { get; }

        /// <summary>
        ///     Application id for the game.
        /// </summary>
        [JsonPropertyName("application_id")]
        public Optional<Snowflake> ApplicationId { get; }

        /// <summary>
        ///     What the player is currently doing.
        /// </summary>
        [JsonPropertyName("details")]
        public Optional<string?> Details { get; }

        /// <summary>
        ///     The user's current party status.
        /// </summary>
        [JsonPropertyName("state")]
        public Optional<string?> State { get; }

        /// <summary>
        ///     The emoji used for a custom status.
        /// </summary>
        [JsonPropertyName("emoji")]
        public Optional<Emoji?> Emoji { get; }

        /// <summary>
        ///     Information for the current party of the player.
        /// </summary>
        [JsonPropertyName("party")]
        public Optional<ActivityParty> Party { get; }

        /// <summary>
        ///     Images for the presence and their hover texts.
        /// </summary>
        [JsonPropertyName("assets")]
        public Optional<ActivityAssets> Assets { get; }

        /// <summary>
        ///     Secrets for Rich Presence joining and spectating.
        /// </summary>
        [JsonPropertyName("secrets")]
        public Optional<ActivitySecrets> Secrets { get; }

        /// <summary>
        ///     Whether or not the activity is an instanced game session.
        /// </summary>
        [JsonPropertyName("instance")]
        public Optional<bool> Instance { get; }

        /// <summary>
        ///     Activity flags ORd together, describes what the payload includes.
        /// </summary>
        [JsonPropertyName("flags")]
        public Optional<int> Flags { get; }

        /// <summary>
        ///     The custom buttons shown in the Rich Presence (max 2).
        /// </summary>
        [JsonPropertyName("buttons")]
        public Optional<string[]> ButtonLabels { get; }
    }
}
