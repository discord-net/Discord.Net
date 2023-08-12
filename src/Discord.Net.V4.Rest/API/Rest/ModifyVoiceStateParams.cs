using Newtonsoft.Json;
using System;

namespace Discord.API.Rest
{
    internal class ModifyVoiceStateParams
    {
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("suppress")]
        public Optional<bool> Suppressed { get; set; }

        [JsonPropertyName("request_to_speak_timestamp")]
        public Optional<DateTimeOffset> RequestToSpeakTimestamp { get; set; }
    }
}
