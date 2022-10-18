using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class AuditLogOptions
    {
        [JsonPropertyName("count")]
        public int? Count { get; set; }
        [JsonPropertyName("channel_id")]
        public ulong? ChannelId { get; set; }
        [JsonPropertyName("message_id")]
        public ulong? MessageId { get; set; }

        //Prune
        [JsonPropertyName("delete_member_days")]
        public int? PruneDeleteMemberDays { get; set; }
        [JsonPropertyName("members_removed")]
        public int? PruneMembersRemoved { get; set; }

        //Overwrite Update
        [JsonPropertyName("role_name")]
        public string OverwriteRoleName { get; set; }
        [JsonPropertyName("type")]
        public PermissionTarget OverwriteType { get; set; }
        [JsonPropertyName("id")]
        public ulong? OverwriteTargetId { get; set; }
    }
}
