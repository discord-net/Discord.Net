namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.FileUpload"/> type.
/// </summary>
public class FileUploadInputComponentInfo : InputComponentInfo
{
    /// <summary>
    ///     Gets the minimum number of values that can be selected.
    /// </summary>
    public int MinValues { get; }

    /// <summary>
    ///     Gets the maximum number of values that can be selected.
    /// </summary>
    public int MaxValues { get; }

    internal FileUploadInputComponentInfo(Builders.FileUploadInputComponentBuilder builder, ModalInfo modal) : base(builder, modal)
    {
        MinValues = builder.MinValues;
        MaxValues = builder.MaxValues;
    }
}
