#pragma warning disable CS1591
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class RequestMembersParams
    {
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("guild_id")]
        private ulong[] _guildIds { get; set; }
        public IEnumerable<ulong> GuildIds { set { _guildIds = value.ToArray(); } }
        public IEnumerable<IGuild> Guilds { set { _guildIds = value.Select(x => x.Id).ToArray(); } }
    }
}
