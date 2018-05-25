using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord.API
{
    internal class AuditLogChange
    {
        [JsonProperty("key")]
        public string ChangedProperty { get; set; }

        [JsonProperty("new_value")]
        public JToken NewValue { get; set; }

        [JsonProperty("old_value")]
        public JToken OldValue { get; set; }
    }
}
