using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Discord;

/// <summary>
///     Represents a class used to build <see cref="OverflowMenuComponent"/>'s.
/// </summary>
/// <remarks>
///     An overflow menu can only be used as the accessory of a <see cref="SectionBuilder"/> or inside an
///     <see cref="ActionRowBuilder"/>; it is not supported by <see cref="ComponentBuilderV2"/>.
/// </remarks>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class OverflowMenuBuilder : IMessageComponentBuilder
{
    /// <summary>
    ///     The maximum number of groups an overflow menu can contain.
    /// </summary>
    public const int MaxGroupCount = 8;

    /// <summary>
    ///     The maximum number of action items an overflow menu can contain in total, across all of its groups.
    /// </summary>
    public const int MaxItemCount = 8;

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
    /// <exception cref="ArgumentException">An overflow menu must contain between 1 and <see cref="MaxGroupCount"/> groups.</exception>
    /// <exception cref="ArgumentException">An overflow menu cannot contain more than <see cref="MaxItemCount"/> action items in total.</exception>
    public OverflowMenuComponent Build()
    {
        Preconditions.NotNull(Groups, nameof(Groups));
        Preconditions.AtLeast(Groups.Count, 1, nameof(Groups), "An overflow menu must contain at least 1 group.");
        Preconditions.AtMost(Groups.Count, MaxGroupCount, nameof(Groups), $"An overflow menu can only contain {MaxGroupCount} groups.");
        Preconditions.AtMost(Groups.Sum(x => x.Components.Count), MaxItemCount, nameof(Groups), $"An overflow menu can only contain {MaxItemCount} action items in total.");

        return new OverflowMenuComponent(Id, Groups.Select(x => x.Build()).ToImmutableArray());
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();

    private string DebuggerDisplay => $"{nameof(OverflowMenuBuilder)}: {Groups.Count} group(s).";
}
