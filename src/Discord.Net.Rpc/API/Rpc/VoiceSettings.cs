#pragma warning disable CS1591

using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class VoiceSettings
    {
        [ModelProperty("input")]
        public VoiceDeviceSettings Input { get; set; }
        [ModelProperty("output")]
        public VoiceDeviceSettings Output { get; set; }
        [ModelProperty("mode")]
        public VoiceMode Mode { get; set; }
        [ModelProperty("automatic_gain_control")]
        public Optional<bool> AutomaticGainControl { get; set; }
        [ModelProperty("echo_cancellation")]
        public Optional<bool> EchoCancellation { get; set; }
        [ModelProperty("noise_suppression")]
        public Optional<bool> NoiseSuppression { get; set; }
        [ModelProperty("qos")]
        public Optional<bool> QualityOfService { get; set; }
        [ModelProperty("silence_warning")]
        public Optional<bool> SilenceWarning { get; set; }
    }
}
