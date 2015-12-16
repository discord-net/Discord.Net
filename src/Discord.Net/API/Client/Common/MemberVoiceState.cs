using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class MemberVoiceState
    {
        [JsonProperty("guild_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong GuildId { get; set; }
        [JsonProperty("user_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong UserId { get; set; }

        [JsonProperty("channel_id"), JsonConverter(typeof(NullableLongStringConverter))]
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
        public bool? IsServerMuted { get; set; }
        [JsonProperty("deaf")]
        public bool? IsServerDeafened { get; set; }
        [JsonProperty("suppress")]
        public bool? IsServerSuppressed { get; set; }
    }
}
