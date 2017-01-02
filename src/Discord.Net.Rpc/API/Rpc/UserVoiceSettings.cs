#pragma warning disable CS1591

using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class UserVoiceSettings
    {
        [JsonProperty("userId")]
        internal ulong UserId { get; set; }
        [JsonProperty("pan")]
        public Optional<Pan> Pan { get; set; }
        [JsonProperty("volume")]
        public Optional<int> Volume { get; set; }
        [JsonProperty("mute")]
        public Optional<bool> Mute { get; set; }
    }
}
