using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an allowed mentions object.
    /// </summary>
    public record AllowedMentions
    {
        /// <summary>
        ///     Creates a <see cref="AllowedMentions"/> with the provided parameters.
        /// </summary>
        /// <param name="parse">An array of allowed mention types to parse from the content.</param>
        /// <param name="roles">Array of role_ids to mention (Max size of 100).</param>
        /// <param name="users">Array of user_ids to mention (Max size of 100).</param>
        /// <param name="repliedUser">For replies, whether to mention the author of the message being replied to (default false).</param>
        [JsonConstructor]
        public AllowedMentions(string[] parse, Snowflake[] roles, Snowflake[] users, bool repliedUser)
        {
            Parse = parse;
            Roles = roles;
            Users = users;
            RepliedUser = repliedUser;
        }

        /// <summary>
        ///     An array of allowed mention types to parse from the content.
        /// </summary>
        [JsonPropertyName("parse")]
        public string[] Parse { get; }

        /// <summary>
        ///     Array of role_ids to mention (Max size of 100).
        /// </summary>
        [JsonPropertyName("roles")]
        public Snowflake[] Roles { get; }

        /// <summary>
        ///     Array of user_ids to mention (Max size of 100).
        /// </summary>
        [JsonPropertyName("users")]
        public Snowflake[] Users { get; }

        /// <summary>
        ///     For replies, whether to mention the author of the message being replied to (default false).
        /// </summary>
        [JsonPropertyName("replied_user")]
        public bool RepliedUser { get; }
    }
}
