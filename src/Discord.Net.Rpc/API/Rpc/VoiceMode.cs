using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class VoiceMode
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("auto_threshold")]
        public bool AutoThreshold { get; set; }
        [JsonProperty("threshold")]
        public float Threshold { get; set; }
        [JsonProperty("shortcut")]
        public VoiceShortcut[] Shortcut { get; set; }
        [JsonProperty("delay")]
        public float Delay { get; set; }
    }
}
