namespace Discord;

public class FileUploadComponentBuilder : IInteractableComponentBuilder
{
    public const int MaxFileCount = 10;

    /// <inheritdoc/>
    public ComponentType Type => ComponentType.FileUpload;

    /// <summary>
    /// 
    /// </summary>
    public string CustomId { get; set; }

    /// <summary>
    ///
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    ///     
    /// </summary>
    public int? MinValues { get; set; }

    /// <summary>
    ///     
    /// </summary>
    public int? MaxValues { get; set; }

    /// <summary>
    ///     
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    ///     
    /// </summary>
    public FileUploadComponentBuilder WithCustomId(string customId)
    {
        CustomId = customId;
        return this;
    }

    /// <summary>
    ///     
    /// </summary>
    public FileUploadComponentBuilder WithMinValues(int? minValues)
    {
        MinValues = minValues;
        return this;
    }

    /// <summary>
    ///     
    /// </summary>
    public FileUploadComponentBuilder WithMaxValues(int? maxValues)
    {
        MaxValues = maxValues;
        return this;
    }

    /// <summary>
    ///     
    /// </summary>
    public FileUploadComponentBuilder WithRequired(bool isRequired)
    {
        IsRequired = isRequired;
        return this;
    }

    public FileUploadComponent Build()
    {
        Preconditions.NotNullOrWhitespace(CustomId, nameof(CustomId));

        if (MinValues is not null && MaxValues is not null)
            Preconditions.AtLeast(MaxValues.Value, MinValues.Value, nameof(MaxValues));

        Preconditions.AtMost(MinValues ?? 0, MaxFileCount, nameof(MinValues));
        Preconditions.AtMost(MaxValues ?? 0, MaxFileCount, nameof(MaxValues));

        return new FileUploadComponent(Id, CustomId, MinValues, MaxValues, IsRequired);
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
