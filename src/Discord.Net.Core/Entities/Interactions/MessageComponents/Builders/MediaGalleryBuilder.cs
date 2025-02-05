using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord;

public class MediaGalleryBuilder : IMessageComponentBuilder
{
    public ComponentType Type => ComponentType.MediaGallery;

    public int? Id { get; set; }

    private List<MediaGalleryItemProperties> _items = new();

    public MediaGalleryBuilder() { }

    public MediaGalleryBuilder(IEnumerable<MediaGalleryItemProperties> items, int? id = null)
    {
        Items = items.ToList();
        Id = id;
    }

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

    public MediaGalleryBuilder AddItem(string url, string description = null, bool isSpoiler = false)
    {
        _items.Add(new MediaGalleryItemProperties(new UnfurledMediaItemProperties(url), description, isSpoiler));
        return this;
    }

    public MediaGalleryBuilder AddItems(params IEnumerable<MediaGalleryItemProperties> items)
    {
        foreach (var item in items)
            _items.Add(item);
        return this;
    }

    public MediaGalleryBuilder WithItems(IEnumerable<MediaGalleryItemProperties> items)
    {
        _items = items.ToList();
        return this;
    }

    public MediaGalleryBuilder WithId(int? id)
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
