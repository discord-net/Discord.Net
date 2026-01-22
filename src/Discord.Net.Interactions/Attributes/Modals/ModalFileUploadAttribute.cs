namespace Discord.Interactions;

/// <summary>
///     Marks a <see cref="IModal"/> property as a file upload input.
/// </summary>
public class ModalFileUploadAttribute : ModalInputAttribute
{
    /// <inheritdoc/>
    public override ComponentType ComponentType => ComponentType.FileUpload;

    /// <summary>
    ///     Get the minimum number of files that can be uploaded.
    /// </summary>
    public int MinValues { get; set; } = 1;

    /// <summary>
    ///     Get the maximum number of files that can be uploaded.
    /// </summary>
    public int MaxValues { get; set; } = 1;

    /// <summary>
    ///     Create a new <see cref="ModalFileUploadAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the file upload component.</param>
    /// <param name="minValues">Minimum number of files that can be uploaded.</param>
    /// <param name="maxValues">Maximum number of files that can be uploaded.</param>
    /// <param name="id">The optional identifier for the component.</param>
    public ModalFileUploadAttribute(string customId, int minValues = 1, int maxValues = 1, int id = 0)
        : base(customId, id)
    {
        MinValues = minValues;
        MaxValues = maxValues;
    }
}
