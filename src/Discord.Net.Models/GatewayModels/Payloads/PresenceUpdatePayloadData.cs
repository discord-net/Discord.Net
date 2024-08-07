using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class PresenceUpdatePayloadData : IPresenceUpdatePayloadData
{
    [JsonPropertyName("since")]
    public int? Since { get; set; }

    [JsonPropertyName("activities")]
    public Activity[] Activities { get; set; } = [];

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonPropertyName("afk")]
    public bool IsAfk { get; set; }

    IEnumerable<IActivityModel> IPresenceUpdatePayloadData.Activities => Activities;
}
