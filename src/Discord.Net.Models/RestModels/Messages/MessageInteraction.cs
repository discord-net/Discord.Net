using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MessageInteraction
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }

    [JsonPropertyName("member")]
    public Optional<GuildMember> Member { get; set; }
}
