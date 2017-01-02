using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class VoiceMode
    {
        [JsonProperty("type")]
        public Optional<string> Type { get; set; }
        [JsonProperty("auto_threshold")]
        public Optional<bool> AutoThreshold { get; set; }
        [JsonProperty("threshold")]
        public Optional<float> Threshold { get; set; }
        [JsonProperty("shortcut")]
        public Optional<VoiceShortcut[]> Shortcut { get; set; }
        [JsonProperty("delay")]
        public Optional<float> Delay { get; set; }
    }
}
