using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a group of action items in an <see cref="OverflowMenuComponent"/>.
/// </summary>
public readonly struct OverflowMenuGroup
{
    /// <summary>
    ///     Gets the label of this group.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if no label is set.
    /// </remarks>
    public string Label { get; }

    /// <summary>
    ///     Gets the action items in this group.
    /// </summary>
    public IReadOnlyCollection<MenuActionItemComponent> Components { get; }

    /// <summary>
    ///     Converts this group to an <see cref="OverflowMenuGroupBuilder"/>.
    /// </summary>
    public OverflowMenuGroupBuilder ToBuilder()
        => new(this);

    internal OverflowMenuGroup(string label, IReadOnlyCollection<MenuActionItemComponent> components)
    {
        Label = label;
        Components = components;
    }
}
