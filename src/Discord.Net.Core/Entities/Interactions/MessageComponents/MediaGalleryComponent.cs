using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a media gallery component.
/// </summary>
public class MediaGalleryComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.MediaGallery;

    /// <inheritdoc/>
    public int? Id { get; }

    /// <summary>
    ///     Gets the items in this media gallery.
    /// </summary>
    public IReadOnlyCollection<MediaGalleryItem> Items { get; }

    /// <summary>
    ///     Converts a <see cref="MediaGalleryComponent"/> to a <see cref="MediaGalleryBuilder"/>.
    /// </summary>
    public MediaGalleryBuilder ToBuilder()
        => new(this);

    internal MediaGalleryComponent(IReadOnlyCollection<MediaGalleryItem> items, int? id)
    {
        Items = items;
        Id = id;
    }

    /// <inheritdoc />
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
