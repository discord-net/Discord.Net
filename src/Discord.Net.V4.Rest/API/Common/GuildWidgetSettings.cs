using System.Text.Json.Serialization;

namespace Discord.API;

internal class GuildWidgetSettings
{
    [JsonPropertyName("enabled")]
    public bool IsEnabled { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }
}
