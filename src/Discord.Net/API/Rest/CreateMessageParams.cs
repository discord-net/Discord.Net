#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateMessageParams
    {
        [JsonProperty("content")]
        internal string _content { get; set; }
        public string Content { set { _content = value; } }

        [JsonProperty("nonce")]
        internal Optional<string> _nonce { get; set; }
        public string Nonce { set { _nonce = value; } }

        [JsonProperty("tts")]
        internal Optional<bool> _tts { get; set; }
        public bool IsTTS { set { _tts = value; } }
    }
}
