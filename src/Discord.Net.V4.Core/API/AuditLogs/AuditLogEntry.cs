using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class AuditLogEntry
{
    [JsonPropertyName("target_id")]
    public ulong? TargetId { get; set; }

    [JsonPropertyName("changes")]
    public Optional<AuditLogChange[]> Changes { get; set; }

    [JsonPropertyName("user_id")]
    public ulong? UserId { get; set; }

    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("action_type")]
    public ActionType Action { get; set; }

    [JsonPropertyName("options")]
    public Optional<AuditLogOptions> Options { get; set; }

    [JsonPropertyName("reason")]
    public Optional<string> Reason { get; set; }
}
