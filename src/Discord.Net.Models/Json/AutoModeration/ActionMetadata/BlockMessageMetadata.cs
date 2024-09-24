using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class BlockMessageMetadata : ActionMetadata, IBlockMessageMetadataModel
{
    [JsonPropertyName("custom_message")]
    public Optional<string> CustomMessage { get; set; }

    string? IBlockMessageMetadataModel.CustomMessage => ~CustomMessage;
}
