namespace Discord;

/// <summary>
///     Represents a separator component.
/// </summary>
public class SeparatorComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Separator;

    /// <inheritdoc/>
    public int? Id { get; }

    /// <summary>
    ///     Gets whether this component is a divider.
    /// </summary>
    public bool? IsDivider { get; }

    /// <summary>
    ///     Gets the spacing of this component.
    /// </summary>
    public SeparatorSpacingSize? Spacing { get; }

    /// <summary>
    ///     Converts a <see cref="SeparatorComponent"/> to a <see cref="SeparatorBuilder"/>.
    /// </summary>
    public SeparatorBuilder ToBuilder()
        => new(this);

    internal SeparatorComponent(bool? isDivider, SeparatorSpacingSize? spacing, int? id = null)
    {
        IsDivider = isDivider;
        Spacing = spacing;
        Id = id;
    }

    /// <inheritdoc />
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
