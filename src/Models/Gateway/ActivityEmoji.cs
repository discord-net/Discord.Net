using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an activity emoji object.
    /// </summary>
    public record ActivityEmoji
    {
        /// <summary>
        ///     Creates a <see cref="ActivityEmoji"/> with the provided parameters.
        /// </summary>
        /// <param name="name">The name of the emoji.</param>
        /// <param name="id">The id of the emoji.</param>
        /// <param name="animated">Whether this emoji is animated.</param>
        [JsonConstructor]
        public ActivityEmoji(string name, Optional<Snowflake> id, Optional<bool> animated)
        {
            Name = name;
            Id = id;
            Animated = animated;
        }

        /// <summary>
        ///     The name of the emoji.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     The id of the emoji.
        /// </summary>
        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; }

        /// <summary>
        ///     Whether this emoji is animated.
        /// </summary>
        [JsonPropertyName("animated")]
        public Optional<bool> Animated { get; }
    }
}
