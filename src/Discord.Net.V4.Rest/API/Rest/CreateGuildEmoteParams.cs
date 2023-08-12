using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreateGuildEmoteParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("image")]
    public Image Image { get; set; }

    [JsonPropertyName("roles")]
    public Optional<ulong[]> RoleIds { get; set; }
}
