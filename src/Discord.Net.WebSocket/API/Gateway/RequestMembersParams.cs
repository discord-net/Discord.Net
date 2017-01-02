#pragma warning disable CS1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class RequestMembersParams
    {
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("guild_id")]
        public IEnumerable<ulong> GuildIds { get; set; }
    }
}
