namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="FileUploadInputComponentInfo"/>.
/// </summary>
public class FileUploadInputComponentBuilder : InputComponentBuilder<FileUploadInputComponentInfo, FileUploadInputComponentBuilder>
{
    protected override FileUploadInputComponentBuilder Instance => this;

    /// <summary>
    ///     Gets and sets the minimum number of files that can be uploaded.
    /// </summary>
    public int MinValues { get; set; } = 1;

    /// <summary>
    ///     Gets and sets the maximum number of files that can be uploaded.
    /// </summary>
    public int MaxValues { get; set; } = 1;

    /// <summary>
    ///     Initializes a new <see cref="FileUploadInputComponentBuilder"/>.
    /// </summary>
    /// <param name="modal"></param>
    public FileUploadInputComponentBuilder(ModalBuilder modal) : base(modal) { }

    /// <summary>
    ///     Sets <see cref="MinValues"/>.
    /// </summary>
    /// <param name="minValues">New value of the <see cref="MinValues"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public FileUploadInputComponentBuilder WithMinValues(int minValues)
    {
        MinValues = minValues;
        return this;
    }

    /// <summary>
    ///     Sets <see cref="MinValues"/>.
    /// </summary>
    /// <param name="minValues">New value of the <see cref="MinValues"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public FileUploadInputComponentBuilder WithMaxValues(int maxValues)
    {
        MaxValues = maxValues;
        return this;
    }

    internal override FileUploadInputComponentInfo Build(ModalInfo modal)
        => new (this, modal);
}
