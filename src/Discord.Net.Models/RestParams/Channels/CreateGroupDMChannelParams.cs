using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateGroupDMChannelParams
{
    [JsonPropertyName("access_tokens")]
    public required string[] AccessTokens { get; set; }

    [JsonPropertyName("nicks")]
    public required Dictionary<ulong, string> Nicks { get; set; }
}
