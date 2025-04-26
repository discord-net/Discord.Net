namespace Discord;

public struct MediaGalleryItemProperties
{
    /// <summary>
    ///     The maximum length of the description.
    /// </summary>
    public const int MaxDescriptionLength = 256;

    /// <summary>
    ///     Gets or sets the media item to display.
    /// </summary>
    public UnfurledMediaItemProperties Media { get; set; }

    /// <summary>
    ///     Gets or sets the description of the media item.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets whether the media item is a spoiler.
    /// </summary>
    public bool IsSpoiler { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MediaGalleryItemProperties"/>.
    /// </summary>
    public MediaGalleryItemProperties() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MediaGalleryItemProperties"/>.
    /// </summary>
    public MediaGalleryItemProperties(UnfurledMediaItemProperties media, string description = null, bool isSpoiler = false)
    {
        Media = media;
        Description = description;
        IsSpoiler = isSpoiler;
    }
}
