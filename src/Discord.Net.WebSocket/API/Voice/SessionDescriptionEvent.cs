using Newtonsoft.Json;

namespace Discord.API.Voice
{
    internal class SessionDescriptionEvent
    {
        [JsonProperty("secret_key")]
        public byte[] SecretKey { get; set; }
        [JsonProperty("mode")]
        public string Mode { get; set; }
        [JsonProperty("dave_protocol_version")]
        public ushort DaveProtocolVersion { get; set; }
    }
}
