using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public sealed class ReadyEvent
    {
        public sealed class ReadState
        {
            [JsonProperty("id")]
            public string ChannelId { get; set; }
            [JsonProperty("mention_count")]
            public int MentionCount { get; set; }
            [JsonProperty("last_message_id")]
            public string LastMessageId { get; set; }
        }

        [JsonProperty("v")]
        public int Version { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("read_state")]
        public ReadState[] ReadStates { get; set; }
        [JsonProperty("guilds")]
        public ExtendedGuild[] Guilds { get; set; }
        [JsonProperty("private_channels")]
        public Channel[] PrivateChannels { get; set; }
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
