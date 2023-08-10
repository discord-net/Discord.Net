using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class RequestMembersParams
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("query")]
        public Optional<string> Query { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
        [JsonProperty("presences")]
        public Optional<bool> Presences { get; set; }
        [JsonProperty("user_ids")]
        public Optional<IEnumerable<ulong>> UserIds { get; set; }
        [JsonProperty("nonce")]
        public Optional<string> Nonce { get; set; }
    }
}
