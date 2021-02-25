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
        [JsonProperty("inviter")]
        public Optional<User> Inviter { get; set; }
        [JsonProperty("approximate_presence_count")]
        public Optional<int?> PresenceCount { get; set; }
        [JsonProperty("approximate_member_count")]
        public Optional<int?> MemberCount { get; set; }
        [JsonProperty("target_user")]
        public Optional<User> TargetUser { get; set; }
        [JsonProperty("target_user_type")]
        public Optional<TargetUserType> TargetUserType { get; set; }
    }
}
