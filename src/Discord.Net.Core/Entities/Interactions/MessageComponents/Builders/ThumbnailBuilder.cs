namespace Discord;

public class ThumbnailComponentBuilder : IMessageComponentBuilder
{
    public ComponentType Type => ComponentType.Thumbnail;

    public int? Id { get; set; }

    public UnfurledMediaItemProperties Media { get; set; }

    public string Description { get; set; }

    public bool IsSpoiler { get; set; } = false;

    public ThumbnailComponentBuilder WithMedia(UnfurledMediaItemProperties media)
    {
        Media = media;
        return this;
    }

    public ThumbnailComponentBuilder WithDescription(string description)
    {
        Description = description;
        return this;
    }

    public ThumbnailComponentBuilder WithId(int id)
    {
        Id = id;
        return this;
    }

    public ThumbnailComponentBuilder WithSpoiler(bool isSpoiler)
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
