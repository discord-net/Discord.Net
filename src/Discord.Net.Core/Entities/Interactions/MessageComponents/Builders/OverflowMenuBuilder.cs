using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Discord;

/// <summary>
///     Represents a class used to build <see cref="OverflowMenuComponent"/>'s.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class OverflowMenuBuilder : IMessageComponentBuilder
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.OverflowMenu;

    /// <inheritdoc/>
    public int? Id { get; set; }

    /// <summary>
    ///     Gets or sets the groups in this overflow menu.
    /// </summary>
    /// <exception cref="ArgumentNullException" accessor="set"><see cref="Groups"/> cannot be null.</exception>
    public List<OverflowMenuGroupBuilder> Groups
    {
        get;
        set => field = value ?? throw new ArgumentNullException(nameof(value), $"{nameof(Groups)} cannot be null.");
    } = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="OverflowMenuBuilder"/>.
    /// </summary>
    public OverflowMenuBuilder() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OverflowMenuBuilder"/>.
    /// </summary>
    /// <param name="groups">The groups in the overflow menu.</param>
    /// <param name="id">The id of the overflow menu.</param>
    public OverflowMenuBuilder(IEnumerable<OverflowMenuGroupBuilder> groups, int? id = null)
    {
        Groups = groups?.ToList() ?? [];
        Id = id;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OverflowMenuBuilder"/> class from an existing <see cref="OverflowMenuComponent"/>.
    /// </summary>
    /// <param name="overflowMenu">The component.</param>
    public OverflowMenuBuilder(OverflowMenuComponent overflowMenu)
    {
        Groups = overflowMenu.Groups?.Select(x => x.ToBuilder()).ToList() ?? [];
        Id = overflowMenu.Id;
    }

    /// <summary>
    ///     Sets the groups in this overflow menu.
    /// </summary>
    /// <param name="groups">The groups to set.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuBuilder WithGroups(IEnumerable<OverflowMenuGroupBuilder> groups)
    {
        Groups = groups.ToList();
        return this;
    }

    /// <summary>
    ///     Adds a group to this overflow menu.
    /// </summary>
    /// <param name="group">The group to add.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuBuilder WithGroup(OverflowMenuGroupBuilder group)
    {
        Groups.Add(group);
        return this;
    }

    /// <summary>
    ///     Adds a group to this overflow menu.
    /// </summary>
    /// <param name="label">The label of the group.</param>
    /// <param name="components">The action items in the group.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuBuilder WithGroup(string label, params MenuActionItemBuilder[] components)
        => WithGroup(new OverflowMenuGroupBuilder(label, components));

    /// <summary>
    ///     Adds a group to this overflow menu.
    /// </summary>
    /// <param name="options">A delegate that configures the group.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuBuilder WithGroup(Action<OverflowMenuGroupBuilder> options)
    {
        var group = new OverflowMenuGroupBuilder();
        options(group);
        return WithGroup(group);
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build"/>
    public OverflowMenuComponent Build()
    {
        Preconditions.NotNull(Groups, nameof(Groups));
        Preconditions.AtLeast(Groups.Count, 1, nameof(Groups), "An overflow menu must contain at least 1 group.");

        return new OverflowMenuComponent(Id, Groups.Select(x => x.Build()).ToImmutableArray());
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();

    private string DebuggerDisplay => $"{nameof(OverflowMenuBuilder)}: {Groups.Count} group(s).";
}
