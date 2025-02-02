using System.Collections.Generic;

namespace Discord;

public class MediaGalleryComponent : IMessageComponent
{
    public ComponentType Type => ComponentType.MediaGallery;

    public int? Id { get; }

    public IReadOnlyCollection<MediaGalleryItem> Items { get; }

    internal MediaGalleryComponent(IReadOnlyCollection<MediaGalleryItem> items, int? id)
    {
        Items = items;
        Id = id;
    }
}
