using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a ban object.
    /// </summary>
    public record Ban
    {
        /// <summary>
        ///     Creates a <see cref="Ban"/> with the provided parameters.
        /// </summary>
        /// <param name="reason">The reason for the ban.</param>
        /// <param name="user">The banned user.</param>
        [JsonConstructor]
        public Ban(string? reason, User user)
        {
            Reason = reason;
            User = user;
        }

        /// <summary>
        ///     The reason for the ban.
        /// </summary>
        [JsonPropertyName("reason")]
        public string? Reason { get; }

        /// <summary>
        ///     The banned user.
        /// </summary>
        [JsonPropertyName("user")]
        public User User { get; }
    }
}
