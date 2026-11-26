using Newtonsoft.Json;
using System.Linq;

namespace Discord.API;

internal class OverflowMenuComponent : IMessageComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("groups")]
    public OverflowMenuGroup[] Groups { get; set; }

    public OverflowMenuComponent() { }

    public OverflowMenuComponent(Discord.OverflowMenuComponent component)
    {
        Type = component.Type;
        Id = component.Id ?? Optional<int>.Unspecified;
        Groups = component.Groups?.Select(x => new OverflowMenuGroup
        {
            Label = x.Label ?? Optional<string>.Unspecified,
            Components = x.Components?.Select(c => new MenuActionItemComponent(c)).ToArray()
        }).ToArray();
    }

    int? IMessageComponent.Id => Id.ToNullable();
    IMessageComponentBuilder IMessageComponent.ToBuilder() => null;
}
