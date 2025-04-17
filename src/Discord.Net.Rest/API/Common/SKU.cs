using Newtonsoft.Json;

namespace Discord.API;

internal class SKU
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("type")]
    public SKUType Type { get; set; }

    [JsonProperty("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("slug")]
    public string Slug { get; set; }

    [JsonProperty("flags")]
    public SKUFlags Flags { get; set; }
}
