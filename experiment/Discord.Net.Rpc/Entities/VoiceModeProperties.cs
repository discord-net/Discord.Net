namespace Discord.Rpc
{
    public class VoiceModeProperties
    {
        public Optional<string> Type { get; set; }
        public Optional<bool> AutoThreshold { get; set; }
        public Optional<float> Threshold { get; set; }
        public Optional<VoiceShortcut[]> Shortcut { get; set; }
        public Optional<float> Delay { get; set; }
    }
}
