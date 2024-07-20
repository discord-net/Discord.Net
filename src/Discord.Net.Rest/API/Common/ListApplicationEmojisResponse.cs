using Newtonsoft.Json;

namespace Discord.API;

internal class ListApplicationEmojisResponse
{
    [JsonProperty("items")]
    public Emoji[] Items { get; set; }
}
