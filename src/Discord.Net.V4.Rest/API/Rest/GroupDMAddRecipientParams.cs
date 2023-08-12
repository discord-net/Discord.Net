using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class GroupDMAddRecipientParams
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("nick")]
    public string Nickname { get; set; }
}
