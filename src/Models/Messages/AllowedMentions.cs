using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord allowed mentions object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#allowed-mentions-object-allowed-mentions-structure"/>
    /// </remarks>
    public record AllowedMentions
    {
        /// <summary>
        /// An array of allowed mention types to parse from the content.
        /// </summary>
        [JsonPropertyName("parse")]
        public AllowedMentionType[]? Parse { get; init; } // Required property candidate

        /// <summary>
        /// Array of <see cref="Role"/> ids to mention (Max size of 100).
        /// </summary>
        [JsonPropertyName("roles")]
        public Snowflake[]? Roles { get; init; } // Required property candidate

        /// <summary>
        /// Array of <see cref="User"/> ids to mention (Max size of 100).
        /// </summary>
        [JsonPropertyName("users")]
        public Snowflake[]? Users { get; init; } // Required property candidate

        /// <summary>
        /// For replies, whether to mention the author of the <see cref="Message"/> being replied to (default false).
        /// </summary>
        [JsonPropertyName("replied_user")]
        public bool RepliedUser { get; init; }
    }
}
