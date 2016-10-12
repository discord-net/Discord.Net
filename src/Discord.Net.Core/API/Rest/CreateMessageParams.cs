#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateMessageParams
    {
        [JsonProperty("content")]
        public string Content { get; }

        [JsonProperty("nonce")]
        public Optional<string> Nonce { get; set; }
        [JsonProperty("tts")]
        public Optional<bool> IsTTS { get; set; }

        public CreateMessageParams(string content)
        {
            Content = content;
        }
    }
}
