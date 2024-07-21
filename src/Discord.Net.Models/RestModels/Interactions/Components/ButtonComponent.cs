using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ComponentType(ComponentTypes.Button)]
public sealed class ButtonComponent : MessageComponent, IModelSource, IButtonComponentModel, IModelSourceOf<IEmoteModel?>
{
    [JsonPropertyName("style")]
    public int Style { get; set; }

    [JsonPropertyName("label")]
    public Optional<string> Label { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<IEmoteModel> Emote { get; set; }

    [JsonPropertyName("custom_id")]
    public Optional<string> CustomId { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }

    [JsonPropertyName("disabled")]
    public Optional<bool> IsDisabled { get; set; }

    string? IButtonComponentModel.Label => ~Label;
    IEmoteModel? IButtonComponentModel.Emote => ~Emote;
    string? IButtonComponentModel.CustomId => ~CustomId;
    string? IButtonComponentModel.Url => ~Url;
    bool? IButtonComponentModel.IsDisabled => ~IsDisabled;

    public IEnumerable<IEntityModel> GetDefinedModels()
    {
        if (Emote.IsSpecified)
            yield return Emote.Value;
    }

    IEmoteModel? IModelSourceOf<IEmoteModel?>.Model => ~Emote;
}
