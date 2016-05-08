using Discord.Net.Converters;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildMemberParams
    {
        [JsonProperty("roles")]
        public ulong[] Roles { get; set; }
        [JsonProperty("mute")]
        public bool Mute { get; set; }
        [JsonProperty("deaf")]
        public bool Deaf { get; set; }
        [JsonProperty("channel_id"), JsonConverter(typeof(UInt64EntityConverter))]
        public IVoiceChannel VoiceChannel { get; set; }
    }
}
