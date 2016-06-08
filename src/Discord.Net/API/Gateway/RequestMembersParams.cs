using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    public class RequestMembersParams
    {
        [JsonProperty("guild_id")]
        public IEnumerable<ulong> GuildIds { get; set; }
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
    }
}
