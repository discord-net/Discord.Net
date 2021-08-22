using Newtonsoft.Json;
using System;

namespace Discord.API.Rest
{
    internal class ModifyVoiceStateParams
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonProperty("suppress")]
        public Optional<bool> Suppressed { get; set; }

        [JsonProperty("request_to_speak_timestamp")]
        public Optional<DateTimeOffset> RequestToSpeakTimestamp { get; set; }
    }
}
