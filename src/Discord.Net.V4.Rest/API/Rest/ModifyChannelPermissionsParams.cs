using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyChannelPermissionsParams
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("allow")]
    public string Allow { get; set; }

    [JsonPropertyName("deny")]
    public string Deny { get; set; }
}
