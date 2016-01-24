using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.VoiceSocket
{
    public class SpeakingEvent
    {
        [JsonProperty("user_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong UserId { get; set; }
        [JsonProperty("ssrc")]
        public uint SSRC { get; set; }
        [JsonProperty("speaking")]
        public bool IsSpeaking { get; set; }
    }
}
