using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyGuildChannelsParams
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }
}
