namespace Discord;

public readonly struct MediaGalleryItem
{
    public UnfurledMediaItem Media { get; }

    public string Description { get; }

    public bool IsSpoiler { get; }

    internal MediaGalleryItem(UnfurledMediaItem media, string description, bool? isSpoiler)
    {
        Media = media;
        Description = description;
        IsSpoiler = isSpoiler ?? false;
    }
}
