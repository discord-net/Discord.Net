#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildParams
    {
        [JsonProperty("username")]
        public Optional<string> Username { get; set; }
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("region")]
        public Optional<string> RegionId { get; set; }
        [JsonProperty("verification_level")]
        public Optional<VerificationLevel> VerificationLevel { get; set; }
        [JsonProperty("default_message_notifications")]
        public Optional<DefaultMessageNotifications> DefaultMessageNotifications { get; set; }
        [JsonProperty("afk_timeout")]
        public Optional<int> AfkTimeout { get; set; }
        [JsonProperty("icon")]
        public Optional<Image?> Icon { get; set; }
        [JsonProperty("splash")]
        public Optional<Image?> Splash { get; set; }
        [JsonProperty("afk_channel_id")]
        public Optional<ulong?> AfkChannelId { get; set; }
        [JsonProperty("owner_id")]
        public Optional<ulong> OwnerId { get; set; }
    }
}
