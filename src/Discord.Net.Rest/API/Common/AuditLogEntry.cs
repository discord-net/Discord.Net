using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class AuditLogEntry
    {
        [JsonPropertyName("target_id")]
        public ulong? TargetId { get; set; }
        [JsonPropertyName("user_id")]
        public ulong? UserId { get; set; }

        [JsonPropertyName("changes")]
        public AuditLogChange[] Changes { get; set; }
        [JsonPropertyName("options")]
        public AuditLogOptions Options { get; set; }

        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("action_type")]
        public ActionType Action { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }
}
