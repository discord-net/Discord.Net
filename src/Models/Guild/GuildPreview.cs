using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a guild preview object.
    /// </summary>
    public record GuildPreview
    {
        /// <summary>
        ///     Creates a <see cref="GuildPreview"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Guild id.</param>
        /// <param name="name">Guild name (2-100 characters).</param>
        /// <param name="icon">Icon hash.</param>
        /// <param name="splash">Splash hash.</param>
        /// <param name="discoverySplash">Discovery splash hash.</param>
        /// <param name="emojis">Custom guild emojis.</param>
        /// <param name="features">Enabled guild features.</param>
        /// <param name="approximateMemberCount">Approximate number of members in this guild.</param>
        /// <param name="approximatePresenceCount">Approximate number of online members in this guild.</param>
        /// <param name="description">The description for the guild, if the guild is discoverable.</param>
        [JsonConstructor]
        public GuildPreview(Snowflake id, string name, string? icon, string? splash, string? discoverySplash, Emoji[] emojis, string[] features, int approximateMemberCount, int approximatePresenceCount, string? description)
        {
            Id = id;
            Name = name;
            Icon = icon;
            Splash = splash;
            DiscoverySplash = discoverySplash;
            Emojis = emojis;
            Features = features;
            ApproximateMemberCount = approximateMemberCount;
            ApproximatePresenceCount = approximatePresenceCount;
            Description = description;
        }

        /// <summary>
        ///     Guild id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     Guild name (2-100 characters).
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     Icon hash.
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; }

        /// <summary>
        ///     Splash hash.
        /// </summary>
        [JsonPropertyName("splash")]
        public string? Splash { get; }

        /// <summary>
        ///     Discovery splash hash.
        /// </summary>
        [JsonPropertyName("discovery_splash")]
        public string? DiscoverySplash { get; }

        /// <summary>
        ///     Custom guild emojis.
        /// </summary>
        [JsonPropertyName("emojis")]
        public Emoji[] Emojis { get; }

        /// <summary>
        ///     Enabled guild features.
        /// </summary>
        [JsonPropertyName("features")]
        public string[] Features { get; }

        /// <summary>
        ///     Approximate number of members in this guild.
        /// </summary>
        [JsonPropertyName("approximate_member_count")]
        public int ApproximateMemberCount { get; }

        /// <summary>
        ///     Approximate number of online members in this guild.
        /// </summary>
        [JsonPropertyName("approximate_presence_count")]
        public int ApproximatePresenceCount { get; }

        /// <summary>
        ///     The description for the guild, if the guild is discoverable.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; }
    }
}
