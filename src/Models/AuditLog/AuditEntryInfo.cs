using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an audit entry info object.
    /// </summary>
    public record AuditEntryInfo
    {
        /// <summary>
        ///     Creates a <see cref="AuditEntryInfo"/> with the provided parameters.
        /// </summary>
        /// <param name="deleteMemberDays">Number of days after which inactive members were kicked.</param>
        /// <param name="membersRemoved">Number of members removed by the prune.</param>
        /// <param name="channelId">Channel in which the entities were targeted.</param>
        /// <param name="messageId">Id of the message that was targeted.</param>
        /// <param name="count">Number of entities that were targeted.</param>
        /// <param name="id">Id of the overwritten entity.</param>
        /// <param name="type">Type of overwritten entity.</param>
        /// <param name="roleName">Name of the role if type is <see cref="AuditEntryInfoType.Role"/> (not present if type is <see cref="AuditEntryInfoType.Member"/>).</param>
        [JsonConstructor]
        public AuditEntryInfo(int? deleteMemberDays, int? membersRemoved, Snowflake? channelId, Snowflake? messageId, int? count, Snowflake? id, AuditEntryInfoType? type, string? roleName)
        {
            DeleteMemberDays = deleteMemberDays;
            MembersRemoved = membersRemoved;
            ChannelId = channelId;
            MessageId = messageId;
            Count = count;
            Id = id;
            Type = type;
            RoleName = roleName;
        }

        /// <summary>
        ///     Number of days after which inactive members were kicked.
        /// </summary>
        [JsonPropertyName("delete_member_days")]
        public int? DeleteMemberDays { get; }

        /// <summary>
        ///     Number of members removed by the prune.
        /// </summary>
        [JsonPropertyName("members_removed")]
        public int? MembersRemoved { get; }

        /// <summary>
        ///     Channel in which the entities were targeted.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; }

        /// <summary>
        ///     Id of the message that was targeted.
        /// </summary>
        [JsonPropertyName("message_id")]
        public Snowflake? MessageId { get; }

        /// <summary>
        ///     Number of entities that were targeted.
        /// </summary>
        [JsonPropertyName("count")]
        public int? Count { get; }

        /// <summary>
        ///     Id of the overwritten entity.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake? Id { get; }

        /// <summary>
        ///     Type of overwritten entity.
        /// </summary>
        [JsonPropertyName("type")]
        public AuditEntryInfoType? Type { get; }

        /// <summary>
        ///     Name of the role if type is <see cref="AuditEntryInfoType.Role"/> (not present if type is <see cref="AuditEntryInfoType.Member"/>).
        /// </summary>
        [JsonPropertyName("role_name")]
        public string? RoleName { get; }
    }
}
