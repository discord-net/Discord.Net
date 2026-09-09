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

    /// <summary>
    ///      Gets the name of this file.
    /// </summary>
    /// <remarks>
    ///      This property is only available when the component is received from Discord. It will be <see langword="null"/> when creating a new component.
    /// </remarks>
    public string Name { get; }

    /// <summary>
    ///     Gets the size of this file in bytes.
    /// </summary>
    /// <remarks>
    ///      This property is only available when the component is received from Discord. It will be <see langword="null"/> when creating a new component.
    /// </remarks>
    public ulong? Size { get; }

    /// <summary>
    ///     Converts a <see cref="FileComponent"/> to a <see cref="FileComponentBuilder"/>.
    /// </summary>
    public FileComponentBuilder ToBuilder()
        => new(this);

    internal FileComponent(UnfurledMediaItem file, bool? isSpoiler, int? id = null, string name = null, ulong? size = null)
    {
        File = file;
        IsSpoiler = isSpoiler;
        Id = id;
        Name = name;
        Size = size;
    }

    /// <inheritdoc />
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
