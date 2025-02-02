namespace Discord;

public class ThumbnailComponent : IMessageComponent
{
    public ComponentType Type => ComponentType.Thumbnail;

    public int? Id { get; }

    public UnfurledMediaItem Media { get; }

    public string Description { get; }

    public bool IsSpoiler { get; }

    internal ThumbnailComponent(int? id, UnfurledMediaItem media, string description, bool? isSpoiler)
    {
        Id = id;
        Media = media;
        Description = description;
        IsSpoiler = isSpoiler ?? false;
    }
}
