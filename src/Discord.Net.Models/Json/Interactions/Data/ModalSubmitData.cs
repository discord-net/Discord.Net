using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModalSubmitData : InteractionData, IModalDataModel
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("components")]
    public required MessageComponent[] Components { get; set; }

    IEnumerable<IMessageComponentModel> IModalDataModel.Components => Components;
}
