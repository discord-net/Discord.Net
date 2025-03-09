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

    internal SeparatorComponent(bool? isDivider, SeparatorSpacingSize? spacing, int? id = null)
    {
        IsDivider = isDivider;
        Spacing = spacing;
        Id = id;
    }
}
