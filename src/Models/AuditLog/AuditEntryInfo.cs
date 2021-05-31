using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an audit entry info object.
    /// </summary>
    public record AuditEntryInfo
    {
        /// <summary>
        ///     Number of days after which inactive members were kicked.
        /// </summary>
        [JsonPropertyName("delete_member_days")]
        public Optional<int> DeleteMemberDays { get; init; } // actually sent as Optional<string>
        
        /// <summary>
        ///     Number of members removed by the prune.
        /// </summary>
        [JsonPropertyName("members_removed")]
        public Optional<int> MembersRemoved { get; init; } // actually sent as Optional<string>

        /// <summary>
        ///     Channel in which the entities were targeted.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; init; }

        /// <summary>
        ///     Id of the message that was targeted.
        /// </summary>
        [JsonPropertyName("message_id")]
        public Optional<Snowflake> MessageId { get; init; }

        /// <summary>
        ///     Number of entities that were targeted.
        /// </summary>
        [JsonPropertyName("count")]
        public Optional<int> Count { get; init; }  // actually sent as Optional<string>

        /// <summary>
        ///     Id of the overwritten entity.
        /// </summary>
        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; init; }

        /// <summary>
        ///     Type of overwritten entity - "0" for "role" or "1" for "member".
        /// </summary>
        [JsonPropertyName("type")]
        public Optional<AuditEntryInfo> Type { get; init; } // actually sent as Optional<string>

        /// <summary>
        ///     Name of the role if type is "0" (not present if type is "1").
        /// </summary>
        [JsonPropertyName("role_name")]
        public Optional<string> RoleName { get; init; }
    }
}
