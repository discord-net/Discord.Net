using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class GuildMembersChunkEvent
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("members")]
        public GuildMember[] Members { get; set; }
        [JsonProperty("chunk_index")]
        public int ChunkIndex { get; set; }
        [JsonProperty("chunk_count")]
        public int ChunkCount { get; set; }
        [JsonProperty("not_found")]
        public Optional<ulong[]> NotFound { get; set; }
        [JsonProperty("presences")]
        public Optional<Presence[]> Presences { get; set; }
        [JsonProperty("nonce")]
        public Optional<string> Nonce { get; set; }
    }
}
