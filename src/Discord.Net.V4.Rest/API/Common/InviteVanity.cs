using System.Text.Json.Serialization;

namespace Discord.API;

public class InviteVanity
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("uses")]
    public int Uses { get; set; }
}
