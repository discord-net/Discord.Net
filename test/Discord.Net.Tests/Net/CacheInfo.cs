using Newtonsoft.Json;

namespace Discord.Net
{
    internal class CacheInfo
    {
        [JsonProperty("guild_id")]
        public ulong? GuildId { get; set; }
        [JsonProperty("version")]
        public uint Version { get; set; }
    }
}