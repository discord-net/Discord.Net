using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a emoji object.
    /// </summary>
    public record Emoji
    {
        /// <summary>
        ///     Creates a <see cref="Emoji"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Emoji id.</param>
        /// <param name="name">Emoji name.</param>
        /// <param name="roles">Roles allowed to use this emoji.</param>
        /// <param name="user">User that created this emoji.</param>
        /// <param name="requireColons">Whether this emoji must be wrapped in colons.</param>
        /// <param name="managed">Whether this emoji is managed.</param>
        /// <param name="animated">Whether this emoji is animated.</param>
        /// <param name="available">Whether this emoji can be used, may be false due to loss of Server Boosts.</param>
        [JsonConstructor]
        public Emoji(Snowflake? id, string? name, Optional<Snowflake[]> roles, Optional<User> user, Optional<bool> requireColons, Optional<bool> managed, Optional<bool> animated, Optional<bool> available)
        {
            Id = id;
            Name = name;
            Roles = roles;
            User = user;
            RequireColons = requireColons;
            Managed = managed;
            Animated = animated;
            Available = available;
        }

        /// <summary>
        ///     Emoji id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake? Id { get; }

        /// <summary>
        ///     Emoji name.
        /// </summary>
        /// <remarks>
        ///     Can be null only in reaction emoji objects.
        /// </remarks>
        [JsonPropertyName("name")]
        public string? Name { get; }

        /// <summary>
        ///     Roles allowed to use this emoji.
        /// </summary>
        [JsonPropertyName("roles")]
        public Optional<Snowflake[]> Roles { get; }

        /// <summary>
        ///     User that created this emoji.
        /// </summary>
        [JsonPropertyName("user")]
        public Optional<User> User { get; }

        /// <summary>
        ///     Whether this emoji must be wrapped in colons.
        /// </summary>
        [JsonPropertyName("require_colons")]
        public Optional<bool> RequireColons { get; }

        /// <summary>
        ///     Whether this emoji is managed.
        /// </summary>
        [JsonPropertyName("managed")]
        public Optional<bool> Managed { get; }

        /// <summary>
        ///     Whether this emoji is animated.
        /// </summary>
        [JsonPropertyName("animated")]
        public Optional<bool> Animated { get; }

        /// <summary>
        ///     Whether this emoji can be used, may be false due to loss of Server Boosts.
        /// </summary>
        [JsonPropertyName("available")]
        public Optional<bool> Available { get; }
    }
}
