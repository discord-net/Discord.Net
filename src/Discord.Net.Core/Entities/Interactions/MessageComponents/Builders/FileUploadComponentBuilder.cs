using System;

namespace Discord;

/// <summary>
///     Represents a class used to build <see cref="FileUploadComponent"/>'s.
/// </summary>
public class FileUploadComponentBuilder : IInteractableComponentBuilder
{
    /// <summary>
    ///     The maximum number of values for the <see cref="FileUploadComponentBuilder.MinValues"/> and <see cref="FileUploadComponentBuilder.MaxValues"/> properties.
    /// </summary>
    public const int MaxFileCount = 10;

    /// <inheritdoc/>
    public ComponentType Type => ComponentType.FileUpload;

    /// <inheritdoc />
    public int? Id { get; set; }

    /// <summary>
    ///     Gets or sets the custom id of the current file upload.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length exceeds <see cref="ModalComponentBuilder.MaxCustomIdLength"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length subceeds 1.</exception>
    public string CustomId
    {
        get => _customId;
        set
        {
            if (value is not null)
            {
                Preconditions.AtLeast(value.Length, 1, nameof(CustomId));
                Preconditions.AtMost(value.Length, ModalComponentBuilder.MaxCustomIdLength, nameof(CustomId));
            }

            _customId = value;
        }
    }

    /// <summary>
    ///     Gets or sets the minimum number of items that must be uploaded (defaults to 1).
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MinValues"/> exceeds <see cref="MaxFileCount"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MinValues"/> length subceeds 0.</exception>
    public int? MinValues
    {
        get => _minValues;
        set
        {
            if (value is not null)
            {
                Preconditions.AtLeast(value.Value, 0, nameof(MinValues));
                Preconditions.AtMost(value.Value, MaxFileCount, nameof(MinValues));
            }

            _minValues = value;
        }
    }

    /// <summary>
    ///     Gets or sets the maximum number of items that can be uploaded (defaults to 1).
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MaxValues"/> exceeds <see cref="MaxFileCount"/>.</exception>
    public int? MaxValues
    {
        get => _maxValues;
        set
        {
            if (value is not null)
            {
                Preconditions.AtMost(value.Value, MaxFileCount, nameof(MaxValues));
            }

            _maxValues = value;
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the current file upload requires files to be uploaded before submitting the modal (defaults to <see langword="true"></see>).
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    ///     Sets the custom id of the current file upload.
    /// </summary>
    /// <param name="customId">The id to use for the current file upload.</param>
    /// <inheritdoc cref="CustomId"/>
    /// <returns>The current builder.</returns>
    public FileUploadComponentBuilder WithCustomId(string customId)
    {
        CustomId = customId;
        return this;
    }

    /// <summary>
    ///     Sets the minimum number of items that must be uploaded (defaults to 1).
    /// </summary>
    /// <param name="minValues">Sets the minimum number of items that must be uploaded.</param>
    /// <inheritdoc cref="MinValues"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileUploadComponentBuilder WithMinValues(int? minValues)
    {
        MinValues = minValues;
        return this;
    }

    /// <summary>
    ///     Sets the maximum number of items that can be uploaded (defaults to 1).
    /// </summary>
    /// <param name="maxValues">The maximum number of items that can be uploaded.</param>
    /// <inheritdoc cref="MaxValues"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileUploadComponentBuilder WithMaxValues(int? maxValues)
    {
        MaxValues = maxValues;
        return this;
    }

    /// <summary>
    ///     Sets whether the current file upload requires files to be uploaded before submitting the modal.
    /// </summary>
    /// <param name="isRequired">Whether the current file upload requires files to be uploaded before submitting the modal.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileUploadComponentBuilder WithRequired(bool isRequired)
    {
        IsRequired = isRequired;
        return this;
    }

    private string _customId;
    private int? _minValues;
    private int? _maxValues;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileUploadComponentBuilder"/>.
    /// </summary>
    public FileUploadComponentBuilder() {}

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileUploadComponentBuilder"/>.
    /// </summary>
    /// <param name="customId">The custom id of the current file upload.</param>
    /// <param name="minValues">The minimum number of items that must be uploaded (defaults to 1).</param>
    /// <param name="maxValues">the maximum number of items that can be uploaded (defaults to 1).</param>
    /// <param name="isRequired">Whether the current file upload requires files to be uploaded before submitting the modal.</param>
    /// <param name="id">The id for the component.</param>
    public FileUploadComponentBuilder(string customId, int? minValues = null, int? maxValues = null, bool isRequired = true, int? id = null)
    {
        CustomId = customId;
        MinValues = minValues;
        MaxValues = maxValues;
        IsRequired = isRequired;
        Id = id;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileUploadComponentBuilder"/> class from an existing <see cref="FileUploadComponent"/>.
    /// </summary>
    /// <param name="fileUpload">The component.</param>
    public FileUploadComponentBuilder(FileUploadComponent fileUpload)
    {
        CustomId = fileUpload.CustomId;
        MinValues = fileUpload.MinValues;
        MaxValues = fileUpload.MaxValues;
        IsRequired = fileUpload.IsRequired;
        Id = fileUpload.Id;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build" />
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
