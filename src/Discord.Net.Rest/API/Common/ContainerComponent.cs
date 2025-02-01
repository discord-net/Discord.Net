using Newtonsoft.Json;

namespace Discord.API;

internal class ContainerComponent : IMessageComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("accent_color")]
    public Optional<int> AccentColor { get; set; }

    [JsonProperty("spoiler")]
    public Optional<bool> IsSpoiler { get; set; }

    [JsonProperty("components")]
    public IMessageComponent[] Components { get; set; }

    public ContainerComponent() { }

    int? IMessageComponent.Id => Id.ToNullable();
}
