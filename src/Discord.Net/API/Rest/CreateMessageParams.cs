using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateMessageParams
    {
        [JsonProperty("content")]
        internal string _content;
        public string Content { set { _content = value; } }

        [JsonProperty("nonce")]
        internal Optional<string> _nonce;
        public string Nonce { set { _nonce = value; } }

        [JsonProperty("tts")]
        internal Optional<bool> _tts;
        public bool IsTTS { set { _tts = value; } }
    }
}
