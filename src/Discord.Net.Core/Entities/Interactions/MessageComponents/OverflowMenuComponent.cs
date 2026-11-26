using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a component of type <see cref="ComponentType.OverflowMenu"/>.
/// </summary>
public class OverflowMenuComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.OverflowMenu;

    /// <inheritdoc/>
    public int? Id { get; }

    /// <summary>
    ///     Gets the groups of action items in this overflow menu.
    /// </summary>
    public IReadOnlyCollection<OverflowMenuGroup> Groups { get; }

    /// <summary>
    ///     Converts a <see cref="OverflowMenuComponent"/> to an <see cref="OverflowMenuBuilder"/>.
    /// </summary>
    public OverflowMenuBuilder ToBuilder()
        => new(this);

    internal OverflowMenuComponent(int? id, IReadOnlyCollection<OverflowMenuGroup> groups)
    {
        Id = id;
        Groups = groups;
    }

    /// <inheritdoc/>
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
