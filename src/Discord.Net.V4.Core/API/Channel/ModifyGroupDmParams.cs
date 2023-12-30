using System.Text.Json.Serialization;

namespace Discord.API;

public class ModifyGroupDmParams : ModifyChannelParams
{
    [JsonPropertyName("icon")]
    public Optional<Image?> Icon { get; set; }
}
