using Newtonsoft.Json;

namespace Discord.API.Client.VoiceSocket
{
    public sealed class SessionDescriptionEvent
    {
        [JsonProperty("secret_key")]
        public byte[] SecretKey { get; set; }
        [JsonProperty("mode")]
        public string Mode { get; set; }
    }
}
