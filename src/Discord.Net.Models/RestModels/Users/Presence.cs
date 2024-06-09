using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Presence
{
    [JsonPropertyName("user")]
    public required User User { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("status")]
    public Optional<string> Status { get; set; }

    [JsonPropertyName("client_status")]
    public Optional<Dictionary<string, string>> ClientStatus { get; set; }

    [JsonPropertyName("activities")]
    public Optional<Activity[]> Activities { get; set; }
}
