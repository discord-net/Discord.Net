using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord optional audit entry info object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/audit-log#audit-log-entry-object-optional-audit-entry-info"/>
    /// </remarks>
    public record OptionalAuditEntryInfo
    {
        /// <summary>
        /// Number of days after which inactive members were kicked.
        /// </summary>
        /// <remarks>
        /// Action type: <see cref="AuditLogEvent.MemberPrune"/>
        /// </remarks>
        [JsonPropertyName("delete_member_days")]
        public Optional<int> DeleteMemberDays { get; init; }

        /// <summary>
        /// Number of members removed by the prune.
        /// </summary>
        /// <remarks>
        /// Action type: <see cref="AuditLogEvent.MemberPrune"/>
        /// </remarks>
        [JsonPropertyName("members_removed")]
        public Optional<int> MembersRemoved { get; init; }

        /// <summary>
        /// Channel in which the entities were targeted.
        /// </summary>
        /// <remarks>
        /// Action type: <see cref="AuditLogEvent.MemberMove"/>, <see cref="AuditLogEvent.MessagePin"/>,
        /// <see cref="AuditLogEvent.MessageUnpin"/>, <see cref="AuditLogEvent.MessageDelete"/>
        /// </remarks>
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; init; }

        /// <summary>
        /// Id of the message that was targeted.
        /// </summary>
        /// <remarks>
        /// Action type: <see cref="AuditLogEvent.MessagePin"/>, <see cref="AuditLogEvent.MessageUnpin"/>
        /// </remarks>
        [JsonPropertyName("message_id")]
        public Optional<Snowflake> MessageId { get; init; }

        /// <summary>
        /// Number of entities that were targeted.
        /// </summary>
        /// <remarks>
        /// Action type: <see cref="AuditLogEvent.MessageDelete"/>, <see cref="AuditLogEvent.MessageBulkDelete"/>,
        /// <see cref="AuditLogEvent.MemberDisconnect"/>, <see cref="AuditLogEvent.MemberMove"/>
        /// </remarks>
        [JsonPropertyName("count")]
        public Optional<int> Count { get; init; }

        /// <summary>
        /// Id of the overwritten entity.
        /// </summary>
        /// <remarks>
        /// Action type: <see cref="AuditLogEvent.ChannelOverwriteCreate"/>, <see cref="AuditLogEvent.ChannelOverwriteDelete"/>,
        /// <see cref="AuditLogEvent.ChannelOverwriteUpdate"/>
        /// </remarks>
        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; init; }

        /// <summary>
        /// Type of overwritten entity.
        /// </summary>
        /// <remarks>
        /// Action type: <see cref="AuditLogEvent.ChannelOverwriteCreate"/>, <see cref="AuditLogEvent.ChannelOverwriteDelete"/>,
        /// <see cref="AuditLogEvent.ChannelOverwriteUpdate"/>
        /// </remarks>
        [JsonPropertyName("type")]
        public Optional<AuditEntryInfoType> Type { get; init; }

        /// <summary>
        /// Name of the role if type is <see cref="AuditEntryInfoType.Role"/> (not present if type is <see cref="AuditEntryInfoType.GuildMember"/>).
        /// </summary>
        /// <remarks>
        /// Action type: <see cref="AuditLogEvent.ChannelOverwriteCreate"/>, <see cref="AuditLogEvent.ChannelOverwriteDelete"/>,
        /// <see cref="AuditLogEvent.ChannelOverwriteUpdate"/>
        /// </remarks>
        [JsonPropertyName("role_name")]
        public Optional<string> RoleName { get; init; }
    }
}
