using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GroupDmAddRecipientParams
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("nick")]
    public required string Nick { get; set; }
}
