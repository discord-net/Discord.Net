using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ButtonComponent
{
    [JsonPropertyName("type")]
    public ComponentType Type { get; set; }

    [JsonPropertyName("style")]
    public ButtonStyle Style { get; set; }

    [JsonPropertyName("label")]
    public Optional<string> Label { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<Emoji> Emote { get; set; }

    [JsonPropertyName("custom_id")]
    public Optional<string> CustomId { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }

    [JsonPropertyName("disabled")]
    public Optional<bool> IsDisabled { get; set; }
}
