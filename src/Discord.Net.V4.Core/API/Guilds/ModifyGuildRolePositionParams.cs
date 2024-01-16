using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyGuildRolePositionParams
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("position")]
    public Optional<int?> Position { get; set; }
}
