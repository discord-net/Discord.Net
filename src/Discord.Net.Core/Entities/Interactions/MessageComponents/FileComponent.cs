namespace Discord;

/// <summary>
///     Represents a file component.
/// </summary>
public class FileComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.File;

    /// <inheritdoc/>
    public int? Id { get; }

    /// <summary>
    ///     Gets the file of this component.
    /// </summary>
    public UnfurledMediaItem File { get; }

    /// <summary>
    ///      Gets whether this file is a spoiler.
    /// </summary>
    public bool? IsSpoiler { get; }

    internal FileComponent(UnfurledMediaItem file, bool? isSpoiler, int? id = null)
    {
        File = file;
        IsSpoiler = isSpoiler;
        Id = id;
    }
}
