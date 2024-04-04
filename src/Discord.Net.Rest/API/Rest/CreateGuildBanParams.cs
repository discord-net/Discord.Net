using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class CreateGuildBanParams
{
    [JsonProperty("delete_message_seconds")]
    public uint DeleteMessageSeconds { get; set; }
}
