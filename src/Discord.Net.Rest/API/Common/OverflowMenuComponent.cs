using Newtonsoft.Json;

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

    int? IMessageComponent.Id => Id.ToNullable();
    IMessageComponentBuilder IMessageComponent.ToBuilder() => null;
}
