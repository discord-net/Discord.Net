using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class ThreadListSyncEvent
    {
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("channel_ids")]
        public Optional<ulong[]> ChannelIds { get; set; }

        [JsonPropertyName("threads")]
        public Channel[] Threads { get; set; }

        [JsonPropertyName("members")]
        public ThreadMember[] Members { get; set; }
    }
}
