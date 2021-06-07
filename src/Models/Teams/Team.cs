using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord team object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/teams#data-models-team-object"/>
    /// </remarks>
    public record Team
    {
        /// <summary>
        /// A hash of the image of the <see cref="Team"/>'s icon.
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; init; }

        /// <summary>
        /// The unique id of the <see cref="Team"/>.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// The members of the <see cref="Team"/>.
        /// </summary>
        [JsonPropertyName("members")]
        public TeamMember[]? Members { get; init; } // Required property candidate

        /// <summary>
        /// The name of the <see cref="Team"/>.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// The <see cref="User"/> id of the current <see cref="Team"/> owner.
        /// </summary>
        [JsonPropertyName("owner_user_id")]
        public Snowflake OwnerUserId { get; init; }
    }
}
