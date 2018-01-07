#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class ExtendedVoiceState
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("voice_state")]
        public Optional<VoiceState> VoiceState { get; set; }
        [JsonProperty("nick")]
        public Optional<string> Nickname { get; set; }
        [JsonProperty("volume")]
        public Optional<int> Volume { get; set; }
        [JsonProperty("mute")]
        public Optional<bool> Mute { get; set; }
        [JsonProperty("pan")]
        public Optional<Pan> Pan { get; set; }
    }
}
