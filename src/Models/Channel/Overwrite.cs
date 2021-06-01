using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a overwrite object.
    /// </summary>
    public record Overwrite
    {
        /// <summary>
        ///     Creates a <see cref="Overwrite"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Role or user id.</param>
        /// <param name="type">Type of entity this overwrite belongs to.</param>
        /// <param name="allow">Permission bit set.</param>
        /// <param name="deny">Permission bit set.</param>
        [JsonConstructor]
        public Overwrite(Snowflake id, OverwriteType type, string allow, string deny)
        {
            Id = id;
            Type = type;
            Allow = allow;
            Deny = deny;
        }

        /// <summary>
        ///     Role or user id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     Type of entity this overwrite belongs to.
        /// </summary>
        [JsonPropertyName("type")]
        public OverwriteType Type { get; }

        /// <summary>
        ///     Permission bit set.
        /// </summary>
        [JsonPropertyName("allow")]
        public string Allow { get; }

        /// <summary>
        ///     Permission bit set.
        /// </summary>
        [JsonPropertyName("deny")]
        public string Deny { get; }
    }
}
