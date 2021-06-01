using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a team object.
    /// </summary>
    public record Team
    {
        /// <summary>
        ///     Creates a <see cref="Team"/> with the provided parameters.
        /// </summary>
        /// <param name="icon">A hash of the image of the team's icon.</param>
        /// <param name="id">The unique id of the team.</param>
        /// <param name="members">The members of the team.</param>
        /// <param name="name">The name of the team.</param>
        /// <param name="ownerUserId">The user id of the current team owner.</param>
        [JsonConstructor]
        public Team(string? icon, Snowflake id, TeamMember[] members, string name, Snowflake ownerUserId)
        {
            Icon = icon;
            Id = id;
            Members = members;
            Name = name;
            OwnerUserId = ownerUserId;
        }

        /// <summary>
        ///     A hash of the image of the team's icon.
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; }

        /// <summary>
        ///     The unique id of the team.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     The members of the team.
        /// </summary>
        [JsonPropertyName("members")]
        public TeamMember[] Members { get; }

        /// <summary>
        ///     The name of the team.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     The user id of the current team owner.
        /// </summary>
        [JsonPropertyName("owner_user_id")]
        public Snowflake OwnerUserId { get; }
    }
}
