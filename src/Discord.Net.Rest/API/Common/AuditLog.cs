using Newtonsoft.Json;

namespace Discord.API
{
    internal class AuditLog
    {
        [JsonProperty("webhooks")]
        public Webhook[] Webhooks { get; set; }

        [JsonProperty("threads")]
        public Channel[] Threads { get; set; }

        [JsonProperty("integrations")]
        public Integration[] Integrations { get; set; }

        [JsonProperty("users")]
        public User[] Users { get; set; }

        [JsonProperty("audit_log_entries")]
        public AuditLogEntry[] Entries { get; set; }
    }
}
