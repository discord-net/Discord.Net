using Newtonsoft.Json;

namespace Discord.API
{
    //TODO: Complete this with all possible values for options
    internal class AuditLogOptions
    {
        //Message delete
        [JsonProperty("count")]
        public int? MessageDeleteCount { get; set; } //TODO: what type of int? (returned as string)
        [JsonProperty("channel_id")]
        public ulong? MessageDeleteChannelId { get; set; }

        //Prune
        [JsonProperty("delete_member_days")]
        public int? PruneDeleteMemberDays { get; set; } //TODO: what type of int? (returned as string)
        [JsonProperty("members_removed")]
        public int? PruneMembersRemoved { get; set; } //TODO: what type of int? (returned as string)

        //Overwrite Update
        [JsonProperty("role_name")]
        public string OverwriteRoleName { get; set; }
        [JsonProperty("type")]
        public string OverwriteType { get; set; }
        [JsonProperty("id")]
        public ulong? OverwriteTargetId { get; set; }
    }
}
