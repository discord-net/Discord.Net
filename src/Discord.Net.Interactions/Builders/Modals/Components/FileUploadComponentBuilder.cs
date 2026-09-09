using System.Collections.Generic;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="FileUploadComponentInfo"/>.
/// </summary>
public class FileUploadComponentBuilder : InputComponentBuilder<FileUploadComponentInfo, FileUploadComponentBuilder>
{
    protected override FileUploadComponentBuilder Instance => this;
    private readonly List<string> _fileTypes = [];

    /// <summary>
    ///     Gets and sets the minimum number of files that can be uploaded.
    /// </summary>
    public int MinValues { get; set; } = 1;

    /// <summary>
    ///     Gets and sets the maximum number of files that can be uploaded.
    /// </summary>
    public int MaxValues { get; set; } = 1;

    /// <summary>
    ///     Gets the allowed file types for this component.
    /// </summary>
    public IReadOnlyCollection<string> FileTypes => _fileTypes.AsReadOnly();

    /// <summary>
    ///     Initializes a new <see cref="FileUploadComponentBuilder"/>.
    /// </summary>
    /// <param name="modal"></param>
    public FileUploadComponentBuilder(ModalBuilder modal) : base(modal) { }

    /// <summary>
    ///     Sets <see cref="MinValues"/>.
    /// </summary>
    /// <param name="minValues">New value of the <see cref="MinValues"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public FileUploadComponentBuilder WithMinValues(int minValues)
    {
        MinValues = minValues;
        return this;
    }

    /// <summary>
    ///     Sets <see cref="MinValues"/>.
    /// </summary>
    /// <param name="maxValues">New value of the <see cref="MaxValues"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public FileUploadComponentBuilder WithMaxValues(int maxValues)
    {
        MaxValues = maxValues;
        return this;
    }

    /// <summary>
    ///     Adds a file type to <see cref="FileTypes"/>.
    /// </summary>
    /// <param name="fileType">The file type to add.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public FileUploadComponentBuilder AddFileType(string fileType)
    {
        _fileTypes.Add(fileType);
        return this;
    }

    internal override FileUploadComponentInfo Build(ModalInfo modal)
        => new (this, modal);
}
