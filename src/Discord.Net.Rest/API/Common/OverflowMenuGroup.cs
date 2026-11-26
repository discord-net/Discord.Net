using Newtonsoft.Json;

namespace Discord.API;

internal class OverflowMenuGroup
{
    [JsonProperty("label")]
    public Optional<string> Label { get; set; }

    [JsonProperty("components")]
    public MenuActionItemComponent[] Components { get; set; }
}
