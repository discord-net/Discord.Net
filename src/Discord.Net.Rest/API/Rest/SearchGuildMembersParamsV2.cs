using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Rest;

internal class SearchGuildMembersParamsV2
{
    [JsonProperty("limit")]
    public Optional<int?> Limit { get; set; }

    [JsonProperty("and_query")]
    public Dictionary<string, string[]> AndQuery { get; set; }

    [JsonProperty("or_query")]
    public Dictionary<string, string[]> OrQuery { get; set; }

    [JsonProperty("after")]
    public SearchParamsAfter After { get; set; }
}

internal class SearchParamsAfter
{
    [JsonProperty("guild_joined_at")]
    public ulong GuildJoinedAt { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }
}
