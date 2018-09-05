using Newtonsoft.Json;

namespace Discord.API
{
    internal class AuditLogOptions
    {
        //Message delete
        [JsonProperty("count")]
        public int? MessageDeleteCount { get; set; }
        [JsonProperty("channel_id")]
        public ulong? MessageDeleteChannelId { get; set; }

        //Prune
        [JsonProperty("delete_member_days")]
        public int? PruneDeleteMemberDays { get; set; }
        [JsonProperty("members_removed")]
        public int? PruneMembersRemoved { get; set; }

        //Overwrite Update
        [JsonProperty("role_name")]
        public string OverwriteRoleName { get; set; }
        [JsonProperty("type")]
        public PermissionTarget OverwriteType { get; set; }
        [JsonProperty("id")]
        public ulong? OverwriteTargetId { get; set; }
    }
}
