using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AuditLogEntry : IAuditLogEntryModel
{
    [JsonPropertyName("target_id")]
    public string? TargetId { get; set; }

    [JsonPropertyName("changes")]
    public Optional<AuditLogChange[]> Changes { get; set; }

    [JsonPropertyName("user_id")]
    public ulong? UserId { get; set; }

    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("action_type")]
    public int ActionType { get; set; }

    [JsonPropertyName("options")]
    public Optional<AuditLogOptions> Options { get; set; }

    [JsonPropertyName("reason")]
    public Optional<string> Reason { get; set; }

    IAuditLogOptionsModel? IAuditLogEntryModel.Options => ~Options;
    string? IAuditLogEntryModel.Reason => ~Reason;
    IEnumerable<IAuditLogChangeModel>? IAuditLogEntryModel.Changes => ~Changes;

}
