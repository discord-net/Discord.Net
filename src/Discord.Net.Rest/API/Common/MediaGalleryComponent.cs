using Newtonsoft.Json;

namespace Discord.API;

internal class MediaGalleryComponent : IMessageComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("items")]
    public MediaGalleryItem[] Items { get; set; }

    public MediaGalleryComponent() { }

    int? IMessageComponent.Id => Id.ToNullable();
}
