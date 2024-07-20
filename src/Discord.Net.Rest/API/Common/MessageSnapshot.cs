using Newtonsoft.Json;

namespace Discord.API;

internal class MessageSnapshot
{
    [JsonProperty("message")]
    public Message Message { get; set; }
}
