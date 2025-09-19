namespace Discord;

public class FileUploadComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.FileUpload;
    /// <summary>
    ///     Gets the ID of this component.
    /// </summary>
    public int? Id { get; }

    /// <summary>
    ///     Gets the custom ID of this component.
    /// </summary>
    public string CustomId { get; }

    /// <summary>
    ///     Gets the minimum number of files a user must upload.
    /// </summary>
    public int? MinValues { get; }

    /// <summary>
    ///     Gets the maximum number of files a user can upload.
    /// </summary>
    public int? MaxValues { get; }

    /// <summary>
    ///     Gets whether this component requires a file upload to be submitted.
    /// </summary>
    public bool IsRequired { get; }

    internal FileUploadComponent(int? id, string customId, int? minValues, int? maxValues, bool isRequired)
    {
        Id = id;
        CustomId = customId;
        MinValues = minValues;
        MaxValues = maxValues;
        IsRequired = isRequired;
    }

    IMessageComponentBuilder IMessageComponent.ToBuilder() => new FileUploadComponentBuilder()
        .WithId(Id)
        .WithCustomId(CustomId)
        .WithMinValues(MinValues)
        .WithMaxValues(MaxValues)
        .WithRequired(IsRequired);
}
