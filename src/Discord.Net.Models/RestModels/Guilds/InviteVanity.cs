using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InviteVanity
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("uses")]
    public int Uses { get; set; }
}
