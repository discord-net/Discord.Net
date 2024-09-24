using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class SendAlertMessageMetadata : ActionMetadata, ISendAlertMessageMetadataModel
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }
}
