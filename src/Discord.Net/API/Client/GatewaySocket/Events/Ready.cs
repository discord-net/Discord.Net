using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public sealed class ReadyEvent
    {
        public sealed class ReadState
        {
            [JsonProperty("id")]
            public string ChannelId { get; }
            [JsonProperty("mention_count")]
            public int MentionCount { get; }
            [JsonProperty("last_message_id")]
            public string LastMessageId { get; }
        }

        [JsonProperty("v")]
        public int Version { get; }
        [JsonProperty("user")]
        public User User { get; }
        [JsonProperty("session_id")]
        public string SessionId { get; }
        [JsonProperty("read_state")]
        public ReadState[] ReadStates { get; }
        [JsonProperty("guilds")]
        public ExtendedGuild[] Guilds { get; }
        [JsonProperty("private_channels")]
        public Channel[] PrivateChannels { get; }
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; }
    }
}
