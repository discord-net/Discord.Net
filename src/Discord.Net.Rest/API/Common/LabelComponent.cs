using Discord.Rest;
using Newtonsoft.Json;

namespace Discord.API;

internal class LabelComponent : IMessageComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; }

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("component")]
    public IMessageComponent Component { get; set; }

    public LabelComponent() {}

    public LabelComponent(Discord.LabelComponent label)
    {
        Type = label.Type;
        Id = label.Id ?? Optional<int>.Unspecified;
        Label = label.Label;
        Description = label.Description;
        Component = label.Component.ToModel();
    }

    public IMessageComponentBuilder ToBuilder() => null;

    [JsonIgnore]
    int? IMessageComponent.Id => Id.ToNullable();
}
