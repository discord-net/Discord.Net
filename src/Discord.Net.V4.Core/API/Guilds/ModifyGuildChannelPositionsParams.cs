using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyGuildChannelPositionsParams
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("position")]
    public Optional<int?> Position { get; set; }

    [JsonPropertyName("lock_permissions")]
    public Optional<bool?> LockPermissions { get; set; }

    [JsonPropertyName("parent_id")]
    public Optional<ulong?> ParentId { get; set; }
}
