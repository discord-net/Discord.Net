using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyCurrentMemberParams
{
    [JsonPropertyName("nick")]
    public Optional<string?> Nickname { get; set; }
}
