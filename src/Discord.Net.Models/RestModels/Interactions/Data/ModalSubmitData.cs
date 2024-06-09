using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[InteractionDataType(InteractionDataTypes.ModalSubmit)]
public sealed class ModalSubmitData : InteractionData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("components")]
    public required MessageComponent[] Components { get; set; }
}
