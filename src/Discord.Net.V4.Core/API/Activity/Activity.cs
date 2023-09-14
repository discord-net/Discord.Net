using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class Activity
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public ActivityType Type { get; set; }

    [JsonPropertyName("url")]
    public Optional<string?> Url { get; set; }

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }

    [JsonPropertyName("timestamps")]
    public Optional<ActivityTimestamps> Timestamps { get; set; }

    [JsonPropertyName("application_id")]
    public Optional<ulong> ApplicationId { get; set; }

    [JsonPropertyName("details")]
    public Optional<string?> Details { get; set; }

    [JsonPropertyName("state")]
    public Optional<string?> State { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<Emoji?> Emoji { get; set; }

    [JsonPropertyName("party")]
    public Optional<ActivityParty> Party { get; set; }

    [JsonPropertyName("assets")]
    public Optional<ActivityAssets> Assets { get; set; }

    [JsonPropertyName("secrets")]
    public Optional<ActivitySecrets> Secrets { get; set; }

    [JsonPropertyName("instance")]
    public Optional<bool> Instance { get; set; }

    [JsonPropertyName("Flags")]
    public Optional<ActivityFlags> Flags { get; set; }

    [JsonPropertyName("buttons")]
    public Optional<ActivityButton[]> Buttons { get; set; }
}
