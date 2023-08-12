using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyGuildWidgetParams
{
    [JsonPropertyName("enabled")]
    public Optional<bool> Enabled { get; set; }

    [JsonPropertyName("channel")]
    public Optional<ulong?> ChannelId { get; set; }
}
