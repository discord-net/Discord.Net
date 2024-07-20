using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class ModifyApplicationEmoteParams
{
    [JsonProperty("name")]
    public Optional<string> Name { get; set; }
}
