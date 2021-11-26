using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class VoiceState
    {
        [JsonProperty("guild_id")]
        public ulong? GuildId { get; set; }
        [JsonProperty("channel_id")]
        public ulong? ChannelId { get; set; }
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        // ALWAYS sent over WebSocket, never on REST
        [JsonProperty("member")]
        public Optional<GuildMember> Member { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("deaf")]
        public bool Deaf { get; set; }
        [JsonProperty("mute")]
        public bool Mute { get; set; }
        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; set; }
        [JsonProperty("self_mute")]
        public bool SelfMute { get; set; }
        [JsonProperty("suppress")]
        public bool Suppress { get; set; }
        [JsonProperty("self_stream")]
        public bool SelfStream { get; set; }
        [JsonProperty("request_to_speak_timestamp")]
        public Optional<DateTimeOffset?> RequestToSpeakTimestamp { get; set; }
    }
}
