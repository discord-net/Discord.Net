using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyCurrentUserNickParams
{
    [JsonPropertyName("nick")]
    public string Nickname { get; set; }
}
