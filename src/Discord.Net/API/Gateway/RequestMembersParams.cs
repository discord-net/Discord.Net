using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord.API.Gateway
{
    public class RequestMembersParams
    {
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("guild_id")]
        public IEnumerable<ulong> GuildIds { get; set; }
        [JsonIgnore]
        public IEnumerable<IGuild> Guilds { set { GuildIds = value.Select(x => x.Id); } }
    }
}
