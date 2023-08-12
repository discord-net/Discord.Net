using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyCurrentMemberParams
{
    [JsonPropertyName("nick")]
    public Optional<string?> Nickname { get; set; }
}
