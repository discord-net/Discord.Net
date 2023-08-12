using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyGuildRolePositionsParams : ModifyGuildRoleParams
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("position")]
    public Optional<int?> Position { get; set; }
}
