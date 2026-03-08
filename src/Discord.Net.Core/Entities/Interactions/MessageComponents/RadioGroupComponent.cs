using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a component of type <see cref="ComponentType.RadioGroup"/>.
/// </summary>
public class RadioGroupComponent : IInteractableComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.RadioGroup;

    /// <summary>
    ///     Gets the ID of this component.
    /// </summary>
    public int? Id { get; }

    /// <summary>
    ///     Gets the custom ID of this component.
    /// </summary>
    public string CustomId { get; }

    /// <summary>
    ///     Gets the options for this radio group.
    /// </summary>
    public IReadOnlyCollection<RadioGroupOption> Options { get; }
    
    /// <summary>
    ///     Gets whether this component requires a file upload to be submitted.
    /// </summary>
    public bool IsRequired { get; }

    internal RadioGroupComponent(int? id, string customId, IReadOnlyCollection<RadioGroupOption> options, bool isRequired)
    {
        Id = id;
        CustomId = customId;
        Options = options;
        IsRequired = isRequired;
    }

    /// <inheritdoc cref="IMessageComponent.ToBuilder"/>
    public RadioGroupBuilder ToBuilder()
        => new(this);

    /// <inheritdoc/>
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
