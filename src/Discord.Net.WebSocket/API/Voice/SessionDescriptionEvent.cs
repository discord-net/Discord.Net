#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Voice
{
    internal class SessionDescriptionEvent
    {
        [JsonProperty("secret_key")]
        public byte[] SecretKey { get; set; }
        [JsonProperty("mode")]
        public string Mode { get; set; }
    }
}
