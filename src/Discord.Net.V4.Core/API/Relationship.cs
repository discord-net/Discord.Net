using System.Text.Json.Serialization;

namespace Discord.API;

public class Relationship
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }

    [JsonPropertyName("type")]
    public RelationshipType Type { get; set; }
}
