using Discord.Rpc;
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class VoiceShortcut
    {
        [JsonProperty("type")]
        public Optional<VoiceShortcutType> Type { get; set; }
        [JsonProperty("code")]
        public Optional<int> Code { get; set; }
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
    }
}
