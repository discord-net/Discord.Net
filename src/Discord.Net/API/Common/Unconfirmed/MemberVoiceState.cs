using Newtonsoft.Json;

namespace Discord.API
{
    public class MemberVoiceState
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }

        [JsonProperty("channel_id")]
        public ulong? ChannelId { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("self_mute")]
        public bool? IsSelfMuted { get; set; }
        [JsonProperty("self_deaf")]
        public bool? IsSelfDeafened { get; set; }
        [JsonProperty("mute")]
        public bool? IsMuted { get; set; }
        [JsonProperty("deaf")]
        public bool? IsDeafened { get; set; }
        [JsonProperty("suppress")]
        public bool? IsSuppressed { get; set; }
    }
}
