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
        public ulong GuildId { get; set; }
        [JsonProperty("user_ids")]
        public IEnumerable<ulong> UserIds { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}
