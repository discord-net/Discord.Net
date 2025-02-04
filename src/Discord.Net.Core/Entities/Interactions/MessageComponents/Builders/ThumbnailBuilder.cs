namespace Discord;

public class ThumbnailBuilder : IMessageComponentBuilder
{
    public ComponentType Type => ComponentType.Thumbnail;

    public int? Id { get; set; }

    public UnfurledMediaItemProperties Media { get; set; }

    public string Description { get; set; }

    public bool IsSpoiler { get; set; } = false;

    public ThumbnailBuilder() { }

    public ThumbnailBuilder(UnfurledMediaItemProperties media, string description = null, bool isSpoiler = false)
    {
        Media = media;
        Description = description;
        IsSpoiler = isSpoiler;
    }

    public ThumbnailBuilder WithMedia(UnfurledMediaItemProperties media)
    {
        Media = media;
        return this;
    }

    public ThumbnailBuilder WithMedia(string url)
    {
        Media = new UnfurledMediaItemProperties(url);
        return this;
    }

    public ThumbnailBuilder WithDescription(string description)
    {
        Description = description;
        return this;
    }

    public ThumbnailBuilder WithId(int id)
    {
        Id = id;
        return this;
    }

    public ThumbnailBuilder WithSpoiler(bool isSpoiler)
    {
        IsSpoiler = isSpoiler;
        return this;
    }

    public ThumbnailComponent Build()
    {
        return new(Id, new UnfurledMediaItem(Media.Url), Description, IsSpoiler);
    }

    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
