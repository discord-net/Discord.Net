using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a reaction object.
    /// </summary>
    public record Reaction
    {
        /// <summary>
        ///     Creates a <see cref="Reaction"/> with the provided parameters.
        /// </summary>
        /// <param name="count">Times this emoji has been used to react.</param>
        /// <param name="me">Whether the current user reacted using this emoji.</param>
        /// <param name="emoji">Emoji information.</param>
        [JsonConstructor]
        public Reaction(int count, bool me, Emoji emoji)
        {
            Count = count;
            Me = me;
            Emoji = emoji;
        }

        /// <summary>
        ///     Times this emoji has been used to react.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; }

        /// <summary>
        ///     Whether the current user reacted using this emoji.
        /// </summary>
        [JsonPropertyName("me")]
        public bool Me { get; }

        /// <summary>
        ///     Emoji information.
        /// </summary>
        [JsonPropertyName("emoji")]
        public Emoji Emoji { get; }
    }
}
