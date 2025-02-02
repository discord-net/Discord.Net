namespace Discord;

public struct MediaGalleryItemProperties
{
    public UnfurledMediaItemProperties Media { get; set; }

    public string Description { get; set; }

    public bool IsSpoiler { get; set; }

    public MediaGalleryItemProperties() { }

    public MediaGalleryItemProperties(UnfurledMediaItemProperties media, string description = null, bool isSpoiler = false)
    {
        Media = media;
        Description = description;
        IsSpoiler = isSpoiler;
    }
}
