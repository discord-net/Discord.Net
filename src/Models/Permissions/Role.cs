using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord role object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/permissions#role-object-role-structure"/>
    /// </remarks>
    public record Role
    {
        /// <summary>
        /// <see cref="Role"/> id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// <see cref="Role"/> name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// <see cref="Color"/> of this <see cref="Role"/>.
        /// </summary>
        [JsonPropertyName("color")]
        public Color Color { get; init; }

        /// <summary>
        /// If this <see cref="Role"/> is pinned in the <see cref="User"/> listing.
        /// </summary>
        [JsonPropertyName("hoist")]
        public bool Hoist { get; init; }

        /// <summary>
        /// Position of this <see cref="Role"/>.
        /// </summary>
        [JsonPropertyName("position")]
        public int Position { get; init; }

        /// <summary>
        /// <see cref="Permissions"/> bit set.
        /// </summary>
        [JsonPropertyName("permissions")]
        public Permissions Permissions { get; init; }

        /// <summary>
        /// Whether this <see cref="Role"/> is managed by an <see cref="Integration"/>.
        /// </summary>
        [JsonPropertyName("managed")]
        public bool Managed { get; init; }

        /// <summary>
        /// Whether this <see cref="Role"/> is mentionable.
        /// </summary>
        [JsonPropertyName("mentionable")]
        public bool Mentionable { get; init; }

        /// <summary>
        /// The tags this <see cref="Role"/> has.
        /// </summary>
        [JsonPropertyName("tags")]
        public Optional<RoleTags> Tags { get; init; }
    }
}
