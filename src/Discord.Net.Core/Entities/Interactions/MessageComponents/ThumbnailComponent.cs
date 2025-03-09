namespace Discord;

/// <summary>
///     Represents a thumbnail component.
/// </summary>
public class ThumbnailComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Thumbnail;

    /// <inheritdoc/>
    public int? Id { get; }

    /// <summary>
    ///     Gets the media of the component.
    /// </summary>
    public UnfurledMediaItem Media { get; }

    /// <summary>
    ///     Gets the description of the component.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets whether the component is a spoiler.
    /// </summary>
    public bool IsSpoiler { get; }

    internal ThumbnailComponent(int? id, UnfurledMediaItem media, string description, bool? isSpoiler)
    {
        Id = id;
        Media = media;
        Description = description;
        IsSpoiler = isSpoiler ?? false;
    }
}
