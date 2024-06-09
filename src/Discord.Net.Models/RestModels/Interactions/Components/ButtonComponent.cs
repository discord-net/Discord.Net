using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ComponentType(ComponentTypes.Button)]
public sealed class ButtonComponent : MessageComponent, IEntityModelSource
{
    [JsonPropertyName("style")]
    public int Style { get; set; }

    [JsonPropertyName("label")]
    public Optional<string> Label { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<IEmote> Emote { get; set; }

    [JsonPropertyName("custom_id")]
    public Optional<string> CustomId { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }

    [JsonPropertyName("disabled")]
    public Optional<bool> IsDisabled { get; set; }

    public IEnumerable<IEntityModel> GetEntities()
    {
        if (Emote.IsSpecified)
            yield return Emote.Value;
    }
}
