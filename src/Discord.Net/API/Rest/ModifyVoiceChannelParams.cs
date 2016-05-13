using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyVoiceChannelParams : ModifyGuildChannelParams
    {
        [JsonProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
    }
}
