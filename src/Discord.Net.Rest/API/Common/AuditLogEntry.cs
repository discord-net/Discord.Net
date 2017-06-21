using Newtonsoft.Json;

namespace Discord.API
{
    internal class AuditLogEntry
    {
        [JsonProperty("target_id")]
        public ulong? TargetId { get; set; }
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }

        [JsonProperty("changes")]
        public AuditLogChange[] Changes { get; set; }
        [JsonProperty("options")]
        public AuditLogOptions Options { get; set; }

        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("action_type")]
        public ActionType Action { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
