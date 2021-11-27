using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class ThreadListSyncEvent
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }

        [JsonProperty("channel_ids")]
        public Optional<ulong[]> ChannelIds { get; set; }

        [JsonProperty("threads")]
        public Channel[] Threads { get; set; }

        [JsonProperty("members")]
        public ThreadMember[] Members { get; set; }
    }
}
