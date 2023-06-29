using System.Text.Json.Serialization;

namespace Discord.API;

internal class Connection
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("revoked")]
    public Optional<bool> Revoked { get; set; }

    [JsonPropertyName("integrations")]
    public Optional<Integration[]> Integrations { get; set; }

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("friend_sync")]
    public bool FriendSync { get; set; }

    [JsonPropertyName("show_activity")]
    public bool ShowActivity { get; set; }

    [JsonPropertyName("two_way_link")]
    public bool TwoWayLink { get; set; }

    [JsonPropertyName("visibility")]
    public ConnectionVisibility Visibility { get; set; }
}
