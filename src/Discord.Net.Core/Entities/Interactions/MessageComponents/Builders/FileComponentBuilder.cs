using System;

namespace Discord;

public class FileComponentBuilder : IMessageComponentBuilder
{
    /// <inheritdoc />
    public ComponentType Type => ComponentType.File;

    /// <inheritdoc />
    public int? Id { get; set; }

    /// <summary>
    ///     Gets or sets the file for the component.
    /// </summary>
    /// <remarks>
    ///     Only attachment URLs are supported.
    /// </remarks>
    public UnfurledMediaItemProperties File { get; set; }

    /// <summary>
    ///     Gets or sets whether the file is a spoiler.
    /// </summary>
    public bool? IsSpoiler { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileComponentBuilder"/>.
    /// </summary>
    public FileComponentBuilder() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileComponentBuilder"/>.
    /// </summary>
    public FileComponentBuilder(UnfurledMediaItemProperties media, bool isSpoiler = false, int? id = null)
    {
        File = media;
        Id = id;
        IsSpoiler = isSpoiler;
    }

    /// <summary>
    ///     Initializes a new <see cref="FileComponentBuilder"/> from existing component.
    /// </summary>
    public FileComponentBuilder(FileComponent file)
    {
        File = file.File.ToProperties();
        IsSpoiler = file.IsSpoiler;
        Id = file.Id;
    }

    /// <summary>
    ///     Sets the file for the component.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileComponentBuilder WithFile(UnfurledMediaItemProperties file)
    {
        File = file;
        return this;
    }

    /// <summary>
    ///     Sets whether the file is a spoiler.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileComponentBuilder WithIsSpoiler(bool? isSpoiler)
    {
        IsSpoiler = isSpoiler;
        return this;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build" />
    public FileComponent Build()
    {
        if (string.IsNullOrWhiteSpace(File.Url))
            throw new InvalidOperationException("File URL must be set.");

        if (!File.Url.StartsWith("attachment://"))
            throw new InvalidOperationException("File URL must be an attachment URL.");

        return new(new UnfurledMediaItem(File.Url), IsSpoiler, Id);
    }

    /// <inheritdoc />
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
