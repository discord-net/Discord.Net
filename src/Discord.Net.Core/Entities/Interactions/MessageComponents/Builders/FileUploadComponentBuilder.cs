using System;
using System.Collections.Generic;
using System.Linq;

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

    /// <summary>
    ///     The maximum number of file types for the <see cref="FileUploadComponentBuilder.FileTypes"/> property.
    /// </summary>
    public const int MaxFileTypeCount = 10;

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
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtLeast(value.Length, 1, nameof(CustomId));
                Preconditions.AtMost(value.Length, ModalComponentBuilder.MaxCustomIdLength, nameof(CustomId));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets the minimum number of items that must be uploaded (defaults to 1).
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MinValues"/> exceeds <see cref="MaxFileCount"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MinValues"/> length subceeds 0.</exception>
    public int? MinValues
    {
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtLeast(value.Value, 0, nameof(MinValues));
                Preconditions.AtMost(value.Value, MaxFileCount, nameof(MinValues));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets the maximum number of items that can be uploaded (defaults to 1).
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="MaxValues"/> exceeds <see cref="MaxFileCount"/>.</exception>
    public int? MaxValues
    {
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtMost(value.Value, MaxFileCount, nameof(MaxValues));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets the allowed file types for the current file upload. Can be <c>image</c>, <c>video</c>, <c>audio</c>, or a file extension (e.g. <c>.png</c>).
    ///     If no file types are specified, all file types are allowed.
    /// </summary>
    public List<string> FileTypes { get; set; } = [];

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

    /// <summary>
    ///     Sets the allowed file types for the current file upload. Can be <c>image</c>, <c>video</c>, <c>audio</c>, or a file extension (e.g. <c>.png</c>).
    /// </summary>
    /// <param name="fileTypes">The allowed file types.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileUploadComponentBuilder WithFileTypes(IEnumerable<string> fileTypes)
    {
        FileTypes = fileTypes?.ToList();
        return this;
    }

    /// <summary>
    ///     Adds a allowed file type for the current file upload. Can be <c>image</c>, <c>video</c>, <c>audio</c>, or a dot prefixed file extension (e.g. <c>.png</c>).
    /// </summary>
    /// <param name="fileType">The allowed file type.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileUploadComponentBuilder AddFileType(string fileType)
    {
        FileTypes ??= [];
        FileTypes.Add(fileType);
        return this;
    }

    /// <summary>
    ///     Adds the allowed file types for the current file upload. Can be <c>image</c>, <c>video</c>, <c>audio</c>, or a dot prefixed file extension (e.g. <c>.png</c>).
    /// </summary>
    /// <param name="fileTypes">The allowed file types.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileUploadComponentBuilder AddFileTypes(params IEnumerable<string> fileTypes)
    {
        FileTypes ??= [];
        FileTypes.AddRange(fileTypes);
        return this;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileUploadComponentBuilder"/>.
    /// </summary>
    public FileUploadComponentBuilder() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileUploadComponentBuilder"/>.
    /// </summary>
    /// <param name="customId">The custom id of the current file upload.</param>
    /// <param name="minValues">The minimum number of items that must be uploaded (defaults to 1).</param>
    /// <param name="maxValues">the maximum number of items that can be uploaded (defaults to 1).</param>
    /// <param name="isRequired">Whether the current file upload requires files to be uploaded before submitting the modal.</param>
    /// <param name="id">The id for the component.</param>
    /// <param name="fileTypes">The allowed file types.</param>
    public FileUploadComponentBuilder(string customId, int? minValues = null, int? maxValues = null, bool isRequired = true, int? id = null, IEnumerable<string> fileTypes = null)
    {
        CustomId = customId;
        MinValues = minValues;
        MaxValues = maxValues;
        IsRequired = isRequired;
        Id = id;
        FileTypes = fileTypes?.ToList();
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
        FileTypes = fileUpload.FileTypes?.ToList();
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build" />
    public FileUploadComponent Build()
    {
        Preconditions.NotNullOrWhitespace(CustomId, nameof(CustomId));

        if (MinValues is not null && MaxValues is not null)
            Preconditions.AtLeast(MaxValues.Value, MinValues.Value, nameof(MaxValues));

        Preconditions.AtMost(MinValues ?? 0, MaxFileCount, nameof(MinValues));
        Preconditions.AtMost(MaxValues ?? 0, MaxFileCount, nameof(MaxValues));

        Preconditions.AtMost(FileTypes?.Count ?? 0, MaxFileTypeCount, nameof(FileTypes));

        foreach (var fileType in FileTypes ?? [])
        {
            if (fileType != "image" &&
                fileType != "video" &&
                fileType != "audio" &&
                !fileType.StartsWith('.'))
                throw new ArgumentException($"Invalid file type: {fileType}. Must be 'image', 'video', 'audio', or start with a '.'", nameof(FileTypes));
        }

        return new FileUploadComponent(Id, CustomId, MinValues, MaxValues, IsRequired, FileTypes);
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
