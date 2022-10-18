using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class Invite
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("guild")]
        public Optional<InviteGuild> Guild { get; set; }
        [JsonPropertyName("channel")]
        public InviteChannel Channel { get; set; }
        [JsonPropertyName("inviter")]
        public Optional<User> Inviter { get; set; }
        [JsonPropertyName("approximate_presence_count")]
        public Optional<int?> PresenceCount { get; set; }
        [JsonPropertyName("approximate_member_count")]
        public Optional<int?> MemberCount { get; set; }
        [JsonPropertyName("target_user")]
        public Optional<User> TargetUser { get; set; }
        [JsonPropertyName("target_user_type")]
        public Optional<TargetUserType> TargetUserType { get; set; }
    }
}
