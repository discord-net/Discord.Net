using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord;

public class MediaGalleryBuilder : IMessageComponentBuilder
{
    /// <summary>
    ///     Gets the maximum number of items that can be added to a media gallery.
    /// </summary>
    public const int MaxItems = 10;

    /// <inheritdoc/>
    public ComponentType Type => ComponentType.MediaGallery;

    /// <inheritdoc/>
    public int? Id { get; set; }

    private List<MediaGalleryItemProperties> _items = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="MediaGalleryBuilder"/>.
    /// </summary>
    public MediaGalleryBuilder() { }

    /// <summary>
    ///    Initializes a new instance of the <see cref="MediaGalleryBuilder"/>.
    /// </summary>
    public MediaGalleryBuilder(params IEnumerable<MediaGalleryItemProperties> items)
    {
        Items = items?.ToList();
    }

    /// <summary>
    ///     Initializes a new <see cref="MediaGalleryBuilder"/> from existing component.
    /// </summary>
    public MediaGalleryBuilder(MediaGalleryComponent mediaGallery)
    {
        Items = mediaGallery.Items.Select(x => x.ToProperties()).ToList();
        Id = mediaGallery.Id;
    }

    /// <summary>
    ///     Gets or sets the items in this media gallery.
    /// </summary>
    public List<MediaGalleryItemProperties> Items
    {
        get => _items;
        set => _items = value;
    }

    /// <summary>
    ///     Adds a new item to the media gallery.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public MediaGalleryBuilder AddItem(MediaGalleryItemProperties item)
    {
        _items.Add(item);
        return this;
    }

    /// <summary>
    ///     Adds a new item to the media gallery.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public MediaGalleryBuilder AddItem(string url, string description = null, bool isSpoiler = false)
    {
        _items.Add(new MediaGalleryItemProperties(new UnfurledMediaItemProperties(url), description, isSpoiler));
        return this;
    }

    /// <summary>
    ///     Adds a list of items to the media gallery.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public MediaGalleryBuilder AddItems(params IEnumerable<MediaGalleryItemProperties> items)
    {
        foreach (var item in items)
            _items.Add(item);
        return this;
    }

    /// <summary>
    ///     Sets the items in the media gallery.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public MediaGalleryBuilder WithItems(IEnumerable<MediaGalleryItemProperties> items)
    {
        _items = items.ToList();
        return this;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build"/>
    public MediaGalleryComponent Build()
    {
        if (_items.Any(x => (x.Description?.Length ?? 0) > MediaGalleryItemProperties.MaxDescriptionLength))
            throw new ArgumentException($"{nameof(MediaGalleryItemProperties)} description length cannot exceed {MediaGalleryItemProperties.MaxDescriptionLength} characters.");

        if (_items.Any(x => !(x.Media.Url?.StartsWith("http://") ?? false)
                            && !(x.Media.Url?.StartsWith("https://") ?? false)
                            && !(x.Media.Url?.StartsWith("attachment://") ?? false)))
            throw new ArgumentException($"{nameof(MediaGalleryItemProperties)} description must be a valid URL or attachment.");

        if (_items.Count is 0 or > MaxItems)
            throw new ArgumentOutOfRangeException(nameof(Items), $"Media gallery items count must be in range [1, {MaxItems}]");

        return new(_items.Select(x => new MediaGalleryItem(new UnfurledMediaItem(x.Media.Url), x.Description, x.IsSpoiler)).ToImmutableArray(), Id);
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
