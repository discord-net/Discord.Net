using Newtonsoft.Json;

namespace Discord.API;

internal class SelectMenuDefaultValue
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("type")]
    public SelectDefaultValueType Type { get; set; }
}
