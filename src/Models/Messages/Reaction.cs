using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord reaction object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#reaction-object-reaction-structure"/>
    /// </remarks>
    public record Reaction
    {
        /// <summary>
        /// Minimum amount of users that reacted in a message.
        /// </summary>
        public const int MinGetReactionsAmount = 1;

        /// <summary>
        /// Maximum amount of users that reacted in a message.
        /// </summary>
        public const int MaxGetReactionsAmount = 100;

        /// <summary>
        /// Times this <see cref="Models.Emoji"/> has been used to react.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; init; }

        /// <summary>
        /// Whether the current user reacted using this <see cref="Models.Emoji"/>.
        /// </summary>
        [JsonPropertyName("me")]
        public bool Me { get; init; }

        /// <summary>
        /// <see cref="Models.Emoji"/> information.
        /// </summary>
        [JsonPropertyName("emoji")]
        public Emoji? Emoji { get; init; } // Required property candidate
    }
}
