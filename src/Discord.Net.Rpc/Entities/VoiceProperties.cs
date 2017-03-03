namespace Discord.Rpc
{
    public class VoiceProperties
    {
        public VoiceDeviceProperties Input { get; set; }
        public VoiceDeviceProperties Output { get; set; }
        public VoiceModeProperties Mode { get; set; }
        public Optional<bool> AutomaticGainControl { get; set; }
        public Optional<bool> EchoCancellation { get; set; }
        public Optional<bool> NoiseSuppression { get; set; }
        public Optional<bool> QualityOfService { get; set; }
        public Optional<bool> SilenceWarning { get; set; }
    }
}
