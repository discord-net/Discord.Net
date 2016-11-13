#pragma warning disable CS1591
using System;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateMessageParams
    {
        [JsonProperty("content")]
        public Optional<string> Content { get; }

        [JsonProperty("nonce")]
        public Optional<string> Nonce { get; set; }
        [JsonProperty("tts")]
        public Optional<bool> IsTTS { get; set; }
        [JsonProperty("embed")]
        public Optional<Embed> Embed { get; set; }

        public CreateMessageParams(string content)
        {
            if (string.IsNullOrEmpty(content))
                Content = null;
            else
                Content = content;
        }
    }
}
