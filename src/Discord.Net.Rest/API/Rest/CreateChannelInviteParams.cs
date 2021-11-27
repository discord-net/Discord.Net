using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateChannelInviteParams
    {
        [JsonProperty("max_age")]
        public Optional<int> MaxAge { get; set; }
        [JsonProperty("max_uses")]
        public Optional<int> MaxUses { get; set; }
        [JsonProperty("temporary")]
        public Optional<bool> IsTemporary { get; set; }
        [JsonProperty("unique")]
        public Optional<bool> IsUnique { get; set; }
        [JsonProperty("target_type")]
        public Optional<TargetUserType> TargetType { get; set; }
        [JsonProperty("target_user_id")]
        public Optional<ulong> TargetUserId { get; set; }
        [JsonProperty("target_application_id")]
        public Optional<ulong> TargetApplicationId { get; set; }
    }
}
