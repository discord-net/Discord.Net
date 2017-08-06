using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class VoiceMode
    {
        [ModelProperty("type")]
        public Optional<string> Type { get; set; }
        [ModelProperty("auto_threshold")]
        public Optional<bool> AutoThreshold { get; set; }
        [ModelProperty("threshold")]
        public Optional<float> Threshold { get; set; }
        [ModelProperty("shortcut")]
        public Optional<VoiceShortcut[]> Shortcut { get; set; }
        [ModelProperty("delay")]
        public Optional<float> Delay { get; set; }
    }
}
