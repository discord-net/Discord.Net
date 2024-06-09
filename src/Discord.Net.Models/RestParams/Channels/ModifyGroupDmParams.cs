using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class ModifyGroupDmParams : ModifyChannelParams
{
    [JsonPropertyName("icon")]
    public Optional<string?> Icon { get; set; }
}
