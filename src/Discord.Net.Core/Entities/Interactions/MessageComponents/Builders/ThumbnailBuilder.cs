using System;

namespace Discord;

public class ThumbnailBuilder : IMessageComponentBuilder
{
    /// <summary>
    ///     Gets the maximum length of the description.
    /// </summary>
    public const int MaxDescriptionLength = 1024;

    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Thumbnail;

    /// <inheritdoc/>
    public int? Id { get; set; }

    /// <summary>
    ///     Gets or sets the media of the thumbnail.
    /// </summary>
    public UnfurledMediaItemProperties Media { get; set; }

    /// <summary>
    ///     Gets or sets the description of the thumbnail.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets whether the thumbnail is a spoiler.
    /// </summary>
    public bool IsSpoiler { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ThumbnailBuilder"/>.
    /// </summary>
    public ThumbnailBuilder() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ThumbnailBuilder"/>.
    /// </summary>
    public ThumbnailBuilder(UnfurledMediaItemProperties media, string description = null, bool isSpoiler = false)
    {
        Media = media;
        Description = description;
        IsSpoiler = isSpoiler;
    }

    /// <summary>
    ///     Sets the media of the thumbnail.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ThumbnailBuilder WithMedia(UnfurledMediaItemProperties media)
    {
        Media = media;
        return this;
    }

    /// <summary>
    ///     Sets the description of the thumbnail.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ThumbnailBuilder WithDescription(string description)
    {
        Description = description;
        return this;
    }

    /// <summary>
    ///     Sets whether the thumbnail is a spoiler.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ThumbnailBuilder WithSpoiler(bool isSpoiler)
    {
        IsSpoiler = isSpoiler;
        return this;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build"/>
    public ThumbnailComponent Build()
    {
        if (Description is not null && Description.Length > MaxDescriptionLength)
            throw new ArgumentException($"Description length must be less than or equal to {MaxDescriptionLength}.", nameof(Description));

        return new(Id, new UnfurledMediaItem(Media.Url), Description, IsSpoiler);
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
