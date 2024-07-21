using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ComponentType(ComponentTypes.SelectMenu)]
public sealed class SelectMenuComponent : MessageComponent, ISelectMenuComponentModel
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("options")]
    public Optional<SelectMenuOption[]> Options { get; set; }

    [JsonPropertyName("channel_types")]
    public Optional<int[]> ChannelTypes { get; set; }

    [JsonPropertyName("placeholder")]
    public Optional<string> Placeholder { get; set; }

    [JsonPropertyName("default_values")]
    public Optional<SelectMenuDefaultValueModel[]> DefaultValues { get; set; }

    [JsonPropertyName("min_values")]
    public Optional<int> MinValues { get; set; }

    [JsonPropertyName("max_values")]
    public Optional<int> MaxValues { get; set; }

    [JsonPropertyName("disabled")]
    public Optional<bool> IsDisabled { get; set; }

    IEnumerable<ISelectMenuOptionModel> ISelectMenuComponentModel.Options => Options | [];

    int[]? ISelectMenuComponentModel.ChannelTypes => ~ChannelTypes;
    string? ISelectMenuComponentModel.Placeholder => ~Placeholder;

    IEnumerable<ISelectMenuDefaultValueModel> ISelectMenuComponentModel.DefaultValues => DefaultValues | [];

    int? ISelectMenuComponentModel.MinValues => ~MinValues;
    int? ISelectMenuComponentModel.MaxValues => ~MaxValues;
    bool? ISelectMenuComponentModel.IsDisabled => ~IsDisabled;
}
