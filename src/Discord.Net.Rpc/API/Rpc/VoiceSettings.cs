#pragma warning disable CS1591

using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class VoiceSettings
    {
        [JsonProperty("input")]
        public VoiceDeviceSettings Input { get; set; }
        [JsonProperty("output")]
        public VoiceDeviceSettings Output { get; set; }
        [JsonProperty("mode")]
        public VoiceMode Mode { get; set; }
        [JsonProperty("automatic_gain_control")]
        public bool AutomaticGainControl { get; set; }
        [JsonProperty("echo_cancellation")]
        public bool EchoCancellation { get; set; }
        [JsonProperty("noise_suppression")]
        public bool NoiseSuppression { get; set; }
        [JsonProperty("qos")]
        public bool QualityOfService { get; set; }
        [JsonProperty("silence_warning")]
        public bool SilenceWarning { get; set; }
    }
}
