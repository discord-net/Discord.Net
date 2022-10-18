using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class AuditLog
    {
        [JsonPropertyName("webhooks")]
        public Webhook[] Webhooks { get; set; }

        [JsonPropertyName("threads")]
        public Channel[] Threads { get; set; }

        [JsonPropertyName("integrations")]
        public Integration[] Integrations { get; set; }

        [JsonPropertyName("users")]
        public User[] Users { get; set; }

        [JsonPropertyName("audit_log_entries")]
        public AuditLogEntry[] Entries { get; set; }
    }
}
