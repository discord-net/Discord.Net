using Newtonsoft.Json;

namespace Discord.API.Voice;

internal class ClientsConnect
{
    [JsonProperty("user_ids")]
    public ulong[] UserIds { get; set; }
}
