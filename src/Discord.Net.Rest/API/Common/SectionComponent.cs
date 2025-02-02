using Discord.Rest;
using Newtonsoft.Json;
using System.Linq;

namespace Discord.API;

internal class SectionComponent : IMessageComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("components")]
    public IMessageComponent[] Components { get; set; }

    [JsonProperty("accessory")]
    public IMessageComponent Accessory { get; set; }

    public SectionComponent() { }

    public SectionComponent(Discord.SectionComponent component)
    {
        Type = component.Type;
        Id = component.Id ?? Optional<int>.Unspecified;
        Components = component.Components.Select(x => x.ToModel()).ToArray();
        Accessory = component.Accessory.ToModel();
    }

    int? IMessageComponent.Id => Id.ToNullable();
}
