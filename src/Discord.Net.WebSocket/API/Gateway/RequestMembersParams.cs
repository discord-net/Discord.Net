#pragma warning disable CS1591
using Discord.Serialization;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    internal class RequestMembersParams
    {
        [ModelProperty("query")]
        public string Query { get; set; }
        [ModelProperty("limit")]
        public int Limit { get; set; }

        [ModelProperty("guild_id")]
        public IEnumerable<ulong> GuildIds { get; set; }
    }
}
