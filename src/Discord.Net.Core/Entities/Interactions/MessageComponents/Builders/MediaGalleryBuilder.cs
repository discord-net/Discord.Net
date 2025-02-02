using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord;

public class MediaGalleryBuilder : IMessageComponentBuilder
{
    public ComponentType Type => ComponentType.MediaGallery;

    public int? Id { get; set; }

    private List<MediaGalleryItemProperties> _items = new();

    public List<MediaGalleryItemProperties> Items
    {
        get => _items;
        set => _items = value;
    }

    public MediaGalleryBuilder AddItem(MediaGalleryItemProperties item)
    {
        _items.Add(item);
        return this;
    }

    public MediaGalleryBuilder WithId(int id)
    {
        Id = id;
        return this;
    }

    public MediaGalleryComponent Build()
    {
        return new(_items.Select(x => new MediaGalleryItem(new UnfurledMediaItem(x.Media.Url), x.Description, x.IsSpoiler)).ToImmutableArray(), Id);
    }

    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
