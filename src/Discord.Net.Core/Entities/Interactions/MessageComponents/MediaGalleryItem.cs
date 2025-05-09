namespace Discord;

/// <summary>
///     Represents a media gallery item.
/// </summary>
public readonly struct MediaGalleryItem
{
    /// <summary>
    ///     Gets the media for this item.
    /// </summary>
    public UnfurledMediaItem Media { get; }

    /// <summary>
    ///     Gets the description for this item.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets whether this item is a spoiler.
    /// </summary>
    public bool IsSpoiler { get; }

    /// <summary>
    ///     Converts a <see cref="MediaGalleryItem"/> to a <see cref="MediaGalleryItemProperties"/>.
    /// </summary>
    public MediaGalleryItemProperties ToProperties()
        => new(Media.ToProperties(), Description, IsSpoiler);

    internal MediaGalleryItem(UnfurledMediaItem media, string description, bool? isSpoiler)
    {
        Media = media;
        Description = description;
        IsSpoiler = isSpoiler ?? false;
    }
}
