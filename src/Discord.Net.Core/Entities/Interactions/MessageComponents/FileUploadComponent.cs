using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord;

/// <summary>
///     Represents a component that allows users to upload files in modals.
/// </summary>
public class FileUploadComponent : IInteractableComponent
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

    /// <summary>
    ///     Gets the allowed file types for this component.
    /// </summary>
    public IReadOnlyCollection<string> FileTypes { get; }

    internal FileUploadComponent(int? id, string customId, int? minValues, int? maxValues, bool isRequired, IEnumerable<string> fileTypes)
    {
        Id = id;
        CustomId = customId;
        MinValues = minValues;
        MaxValues = maxValues;
        IsRequired = isRequired;
        FileTypes = fileTypes?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
    }

    /// <inheritdoc cref="IMessageComponent.ToBuilder"/>
    public FileUploadComponentBuilder ToBuilder()
        => new(this);

    /// <inheritdoc/>
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
