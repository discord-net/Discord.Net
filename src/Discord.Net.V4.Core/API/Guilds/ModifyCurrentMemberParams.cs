using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyCurrentMemberParams
{
    [JsonPropertyName("nick")]
    public Optional<string?> Nickname { get; set; }
}
