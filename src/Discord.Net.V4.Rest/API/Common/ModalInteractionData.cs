using System.Text.Json.Serialization;

namespace Discord.API;

internal class ModalInteractionData : IDiscordInteractionData
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("components")]
    public ActionRowComponent[] Components { get; set; }
}
