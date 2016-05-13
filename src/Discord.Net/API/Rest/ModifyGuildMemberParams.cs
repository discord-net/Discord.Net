using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildMemberParams
    {
        [JsonProperty("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [JsonProperty("mute")]
        public Optional<bool> Mute { get; set; }
        [JsonProperty("deaf")]
        public Optional<bool> Deaf { get; set; }
        [JsonProperty("nick")]
        public Optional<string> Nickname { get; set; }
        [JsonProperty("channel_id")]
        public Optional<IVoiceChannel> VoiceChannel { get; set; }
    }
}
