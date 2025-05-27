using Newtonsoft.Json;

namespace Discord.API.Voice;
internal class ClientDisconnectEvent
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }
}
