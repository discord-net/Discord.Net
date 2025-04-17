using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class CreateApplicationEmoteParams
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("image")]
    public Image Image { get; set; }
}
