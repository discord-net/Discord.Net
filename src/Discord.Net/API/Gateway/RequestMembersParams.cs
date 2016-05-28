using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class RequestMembersParams
    {
        [JsonProperty("guild_id")]
        public ulong[] GuildId { get; set; }
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
    }
}
