#pragma warning disable CS1591

using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class VoiceSettings
    {
        [JsonProperty("input")]
        public VoiceDeviceSettings Input { get; set; }
        [JsonProperty("output")]
        public VoiceDeviceSettings Output { get; set; }
        [JsonProperty("mode")]
        public VoiceMode Mode { get; set; }
        [JsonProperty("automatic_gain_control")]
        public Optional<bool> AutomaticGainControl { get; set; }
        [JsonProperty("echo_cancellation")]
        public Optional<bool> EchoCancellation { get; set; }
        [JsonProperty("noise_suppression")]
        public Optional<bool> NoiseSuppression { get; set; }
        [JsonProperty("qos")]
        public Optional<bool> QualityOfService { get; set; }
        [JsonProperty("silence_warning")]
        public Optional<bool> SilenceWarning { get; set; }
    }
}
