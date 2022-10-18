using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class RequestMembersParams
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("guild_id")]
        public IEnumerable<ulong> GuildIds { get; set; }
    }
}
