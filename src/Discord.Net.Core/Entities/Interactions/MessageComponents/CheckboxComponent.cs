namespace Discord;

/// <summary>
///     Represents a message component of type <see cref="ComponentType.Checkbox"/>.
/// </summary>
public class CheckboxComponent : IInteractableComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Checkbox;

    /// <summary>
    ///     Gets the ID of this component.
    /// </summary>
    public int? Id { get; }

    /// <summary>
    ///     Gets the custom ID of this component.
    /// </summary>
    public string CustomId { get; }

    /// <summary>
    ///     Gets the default state of this checkbox.
    /// </summary>
    public bool DefaultState { get; }

    internal CheckboxComponent(int? id, string customId, bool defaultState)
    {
        Id = id;
        CustomId = customId;
        DefaultState = defaultState;
    }

    /// <inheritdoc cref="IMessageComponent.ToBuilder"/>
    public CheckboxBuilder ToBuilder()
        => new(this);

    /// <inheritdoc/>
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
