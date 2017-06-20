using Newtonsoft.Json;

namespace Discord.API
{
    internal class AuditLog
    {
        //TODO: figure out how this works
        //[JsonProperty("webhooks")]
        //public object Webhooks { get; set; }

        [JsonProperty("users")]
        public User[] Users { get; set; }

        [JsonProperty("audit_log_entries")]
        public AuditLogEntry[] Entries { get; set; }
    }
}
