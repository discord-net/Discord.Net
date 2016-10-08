using Discord.Rpc;
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class VoiceShortcut
    {
        [JsonProperty("type")]
        public VoiceShortcutType Type { get; set; }
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
