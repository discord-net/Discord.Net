using Newtonsoft.Json;

namespace Discord.API;

internal class CheckboxGroupOption
{
    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("description")]
    public Optional<string> Description { get; set; }

    [JsonProperty("default")]
    public Optional<bool> DefaultState { get; set; }
}
