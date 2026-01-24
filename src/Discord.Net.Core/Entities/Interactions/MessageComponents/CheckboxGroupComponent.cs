using System.Collections.Generic;

namespace Discord;

public class CheckboxGroupComponent : IInteractableComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.CheckboxGroup;

    /// <summary>
    ///     Gets the ID of this component.
    /// </summary>
    public int? Id { get; }

    /// <summary>
    ///     Gets the custom ID of this component.
    /// </summary>
    public string CustomId { get; }

    /// <summary>
    ///     
    /// </summary>
    public IReadOnlyCollection<CheckboxGroupOption> Options { get; }

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

    internal CheckboxGroupComponent(int? id, string customId, IReadOnlyCollection<CheckboxGroupOption> options, int? minValues, int? maxValues, bool isRequired)
    {
        Id = id;
        CustomId = customId;
        Options = options;
        MinValues = minValues;
        MaxValues = maxValues;
        IsRequired = isRequired;
    }

    /// <inheritdoc cref="IMessageComponent.ToBuilder"/>
    public CheckboxGroupBuilder ToBuilder()
        => new(this);

    /// <inheritdoc/>
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
