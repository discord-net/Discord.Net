#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class Invite
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("guild")]
        public Optional<InviteGuild> Guild { get; set; }
        [JsonProperty("channel")]
        public InviteChannel Channel { get; set; }
        [JsonProperty("approximate_presence_count")]
        public Optional<int?> PresenceCount { get; set; }
        [JsonProperty("approximate_member_count")]
        public Optional<int?> MemberCount { get; set; }
    }
}
