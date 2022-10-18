using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class Overwrite
    {
        [JsonPropertyName("id")]
        public ulong TargetId { get; set; }
        [JsonPropertyName("type")]
        public PermissionTarget TargetType { get; set; }
        [JsonPropertyName("deny"), Int53]
        public string Deny { get; set; }
        [JsonPropertyName("allow"), Int53]
        public string Allow { get; set; }
    }
}
