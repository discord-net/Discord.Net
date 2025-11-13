namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="FileUploadComponentInfo"/>.
/// </summary>
public class FileUploadComponentBuilder : InputComponentBuilder<FileUploadComponentInfo, FileUploadComponentBuilder>
{
    protected override FileUploadComponentBuilder Instance => this;

    /// <summary>
    ///     Gets and sets the minimum number of files that can be uploaded.
    /// </summary>
    public int MinValues { get; set; } = 1;

    /// <summary>
    ///     Gets and sets the maximum number of files that can be uploaded.
    /// </summary>
    public int MaxValues { get; set; } = 1;

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

    internal override FileUploadComponentInfo Build(ModalInfo modal)
        => new (this, modal);
}
