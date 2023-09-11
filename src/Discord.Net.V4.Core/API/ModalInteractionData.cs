using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModalInteractionData
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("components")]
    public ActionRowComponent[] Components { get; set; }
}
