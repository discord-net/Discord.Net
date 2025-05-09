using Discord.Rest;
using Newtonsoft.Json;
using System.Linq;

namespace Discord.API;

internal class ContainerComponent : IMessageComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("accent_color")]
    public Optional<uint?> AccentColor { get; set; }

    [JsonProperty("spoiler")]
    public Optional<bool> IsSpoiler { get; set; }

    [JsonProperty("components")]
    public IMessageComponent[] Components { get; set; }

    public ContainerComponent() { }

    public ContainerComponent(Discord.ContainerComponent component)
    {
        Type = component.Type;
        Id = component.Id ?? Optional<int>.Unspecified;
        AccentColor = component.AccentColor?.RawValue ?? Optional<uint?>.Unspecified;
        IsSpoiler = component.IsSpoiler ?? Optional<bool>.Unspecified;
        Components = component.Components.Select(x => x.ToModel()).ToArray();
    }

    int? IMessageComponent.Id => Id.ToNullable();
}
