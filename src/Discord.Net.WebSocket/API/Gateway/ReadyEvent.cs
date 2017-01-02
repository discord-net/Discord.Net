#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class ReadyEvent
    {
        public class ReadState
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
        [JsonProperty("relationships")]
        public Relationship[] Relationships { get; set; }

        //Ignored
        /*[JsonProperty("user_settings")]
        [JsonProperty("user_guild_settings")]
        [JsonProperty("tutorial")]*/
    }
}
