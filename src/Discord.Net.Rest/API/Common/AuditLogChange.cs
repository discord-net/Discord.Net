using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Discord.API
{
    internal class AuditLogChange
    {
        [JsonPropertyName("key")]
        public string ChangedProperty { get; set; }

        [JsonPropertyName("new_value")]
        public JToken NewValue { get; set; }

        [JsonPropertyName("old_value")]
        public JToken OldValue { get; set; }
    }
}
