using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public abstract class ModifyChannelParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }
}
