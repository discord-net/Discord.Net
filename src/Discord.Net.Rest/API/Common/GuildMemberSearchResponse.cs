using Newtonsoft.Json;

namespace Discord.API;

internal class GuildMemberSearchResponse
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("members")]
    public SupplementalGuildUser[] Members { get; set; }

    [JsonProperty("page_result_count")]
    public int PageResultCount { get; set; }

    [JsonProperty("total_result_count")]
    public int TotalResultCount { get; set; }
}
