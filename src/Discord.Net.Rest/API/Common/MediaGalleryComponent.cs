using Discord.Rest;
using Newtonsoft.Json;
using System.Linq;

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

    public MediaGalleryComponent(Discord.MediaGalleryComponent component)
    {
        Type = component.Type;
        Id = component.Id ?? Optional<int>.Unspecified;
        Items = component.Items.Select(x => new MediaGalleryItem
        {
            Description = x.Description,
            IsSpoiler = x.IsSpoiler,
            Media = x.Media.ToModel()
        }).ToArray();
    }

    int? IMessageComponent.Id => Id.ToNullable();
}
