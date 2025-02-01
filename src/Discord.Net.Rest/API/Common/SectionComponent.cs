using Newtonsoft.Json;

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

    int? IMessageComponent.Id => Id.ToNullable();
}
