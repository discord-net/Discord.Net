using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildWidgetSettings
{
    [JsonPropertyName("enabled")]
    public bool IsEnabled { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }
}
