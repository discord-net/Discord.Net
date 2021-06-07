using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord team member object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/teams#data-models-team-members-object"/>
    /// </remarks>
    public record TeamMember
    {
        /// <summary>
        /// The <see cref="User"/>'s <see cref="Models.MembershipState"/> on the <see cref="Team"/>.
        /// </summary>
        [JsonPropertyName("membership_state")]
        public MembershipState MembershipState { get; init; }

        /// <summary>
        /// Will always be ["*"].
        /// </summary>
        [JsonPropertyName("permissions")]
        public string[]? Permissions { get; init; } // Required property candidate

        /// <summary>
        /// The id of the parent <see cref="Team"/> of which they are a <see cref="TeamMember"/>.
        /// </summary>
        [JsonPropertyName("team_id")]
        public Snowflake TeamId { get; init; }

        /// <summary>
        /// The avatar, discriminator, id, and username of the <see cref="Models.User"/>.
        /// </summary>
        [JsonPropertyName("user")]
        public User? User { get; init; } // Required property candidate
    }
}
