using Newtonsoft.Json;

namespace Discord.API;

internal class BulkBanResult
{
    [JsonProperty("banned_users")]
    public ulong[] BannedUsers { get; set; }

    [JsonProperty("failed_users")]
    public ulong[] FailedUsers { get; set; }
}
