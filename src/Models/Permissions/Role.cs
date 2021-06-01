using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a role object.
    /// </summary>
    public record Role
    {
        /// <summary>
        ///     Creates a <see cref="Role"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Role id.</param>
        /// <param name="name">Role name.</param>
        /// <param name="color">Integer representation of hexadecimal color code.</param>
        /// <param name="hoist">If this role is pinned in the user listing.</param>
        /// <param name="position">Position of this role.</param>
        /// <param name="permissions">Permission bit set.</param>
        /// <param name="managed">Whether this role is managed by an integration.</param>
        /// <param name="mentionable">Whether this role is mentionable.</param>
        /// <param name="tags">The tags this role has.</param>
        [JsonConstructor]
        public Role(Snowflake id, string name, int color, bool hoist, int position, Permissions permissions, bool managed, bool mentionable, Optional<RoleTags> tags)
        {
            Id = id;
            Name = name;
            Color = color;
            Hoist = hoist;
            Position = position;
            Permissions = permissions;
            Managed = managed;
            Mentionable = mentionable;
            Tags = tags;
        }

        /// <summary>
        ///     Role id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     Role name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     Integer representation of hexadecimal color code.
        /// </summary>
        [JsonPropertyName("color")]
        public int Color { get; }

        /// <summary>
        ///     If this role is pinned in the user listing.
        /// </summary>
        [JsonPropertyName("hoist")]
        public bool Hoist { get; }

        /// <summary>
        ///     Position of this role.
        /// </summary>
        [JsonPropertyName("position")]
        public int Position { get; }

        /// <summary>
        ///     Permission bit set.
        /// </summary>
        [JsonPropertyName("permissions")]
        public Permissions Permissions { get; }

        /// <summary>
        ///     Whether this role is managed by an integration.
        /// </summary>
        [JsonPropertyName("managed")]
        public bool Managed { get; }

        /// <summary>
        ///     Whether this role is mentionable.
        /// </summary>
        [JsonPropertyName("mentionable")]
        public bool Mentionable { get; }

        /// <summary>
        ///     The tags this role has.
        /// </summary>
        [JsonPropertyName("tags")]
        public Optional<RoleTags> Tags { get; }
    }
}
