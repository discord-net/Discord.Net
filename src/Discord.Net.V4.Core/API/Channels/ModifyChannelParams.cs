using System.Text.Json.Serialization;

namespace Discord.API;

public abstract class ModifyChannelParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }
}
