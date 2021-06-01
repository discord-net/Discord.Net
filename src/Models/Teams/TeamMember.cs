using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a team member object.
    /// </summary>
    public record TeamMember
    {
        /// <summary>
        ///     Creates a <see cref="TeamMember"/> with the provided parameters.
        /// </summary>
        /// <param name="membershipState">The user's membership state on the team.</param>
        /// <param name="permissions">Will always be ["*"].</param>
        /// <param name="teamId">The id of the parent team of which they are a member.</param>
        /// <param name="user">The avatar, discriminator, id, and username of the user.</param>
        [JsonConstructor]
        public TeamMember(MembershipState membershipState, string[] permissions, Snowflake teamId, User user)
        {
            MembershipState = membershipState;
            Permissions = permissions;
            TeamId = teamId;
            User = user;
        }

        /// <summary>
        ///     The user's membership state on the team.
        /// </summary>
        [JsonPropertyName("membership_state")]
        public MembershipState MembershipState { get; }

        /// <summary>
        ///     Will always be ["*"].
        /// </summary>
        [JsonPropertyName("permissions")]
        public string[] Permissions { get; }

        /// <summary>
        ///     The id of the parent team of which they are a member.
        /// </summary>
        [JsonPropertyName("team_id")]
        public Snowflake TeamId { get; }

        /// <summary>
        ///     The avatar, discriminator, id, and username of the user.
        /// </summary>
        [JsonPropertyName("user")]
        public User User { get; }
    }
}
