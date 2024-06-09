using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyCurrentUserParams
{
    [JsonPropertyName("username")]
    public Optional<string> Username { get; set; }

    [JsonPropertyName("avatar")]
    public Optional<string?> Avatar { get; set; }
}
