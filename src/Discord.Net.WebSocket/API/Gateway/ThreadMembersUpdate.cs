using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class ThreadMembersUpdated
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }

        [JsonProperty("member_count")]
        public int MemberCount { get; set; }

        [JsonProperty("added_members")]
        public Optional<ThreadMember[]> AddedMembers { get; set; }

        [JsonProperty("removed_member_ids")]
        public Optional<ulong[]> RemovedMemberIds { get; set; }
    }
}
