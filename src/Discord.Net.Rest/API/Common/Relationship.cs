using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class Relationship
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("user")]
        public User User { get; set; }
        [JsonPropertyName("type")]
        public RelationshipType Type { get; set; }
    }
}
