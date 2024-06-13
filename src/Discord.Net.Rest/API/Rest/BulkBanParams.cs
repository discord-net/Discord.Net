using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class BulkBanParams
{
    [JsonProperty("user_ids")]
    public ulong[] UserIds { get; set; }

    [JsonProperty("delete_message_seconds")]
    public Optional<int> DeleteMessageSeconds { get; set; }
}
