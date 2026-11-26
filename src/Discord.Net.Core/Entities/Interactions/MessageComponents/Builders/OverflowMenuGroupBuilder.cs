using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord;

/// <summary>
///     Represents a class used to build <see cref="OverflowMenuGroup"/>'s.
/// </summary>
public class OverflowMenuGroupBuilder
{
    /// <summary>
    ///     The maximum length of the group label.
    /// </summary>
    public const int MaxLabelLength = 80;

    /// <summary>
    ///     The maximum number of action items a group can contain.
    /// </summary>
    public const int MaxChildCount = 8;

    /// <summary>
    ///     Gets or sets the label of this group.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Label"/> length exceeds <see cref="MaxLabelLength"/>.</exception>
    public string Label
    {
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtMost(value.Length, MaxLabelLength, nameof(Label));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets the action items in this group.
    /// </summary>
    /// <exception cref="ArgumentNullException" accessor="set"><see cref="Components"/> cannot be null.</exception>
    public List<MenuActionItemBuilder> Components
    {
        get;
        set => field = value ?? throw new ArgumentNullException(nameof(value), $"{nameof(Components)} cannot be null.");
    } = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="OverflowMenuGroupBuilder"/>.
    /// </summary>
    public OverflowMenuGroupBuilder() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OverflowMenuGroupBuilder"/>.
    /// </summary>
    /// <param name="label">The label of the group.</param>
    /// <param name="components">The action items in the group.</param>
    public OverflowMenuGroupBuilder(string label, IEnumerable<MenuActionItemBuilder> components = null)
    {
        Label = label;
        Components = components?.ToList() ?? [];
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OverflowMenuGroupBuilder"/> class from an existing <see cref="OverflowMenuGroup"/>.
    /// </summary>
    /// <param name="group">The group.</param>
    public OverflowMenuGroupBuilder(OverflowMenuGroup group)
    {
        Label = group.Label;
        Components = group.Components?.Select(x => x.ToBuilder()).ToList() ?? [];
    }

    /// <summary>
    ///     Sets the label of this group.
    /// </summary>
    /// <param name="label">The label to set.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuGroupBuilder WithLabel(string label)
    {
        Label = label;
        return this;
    }

    /// <summary>
    ///     Sets the action items in this group.
    /// </summary>
    /// <param name="components">The action items to set.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuGroupBuilder WithComponents(IEnumerable<MenuActionItemBuilder> components)
    {
        Components = components.ToList();
        return this;
    }

    /// <summary>
    ///     Adds an action item to this group.
    /// </summary>
    /// <param name="component">The action item to add.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuGroupBuilder WithMenuActionItem(MenuActionItemBuilder component)
    {
        Components.Add(component);
        return this;
    }

    /// <summary>
    ///     Adds an action item to this group.
    /// </summary>
    /// <param name="label">The label of the action item.</param>
    /// <param name="customId">The custom id of the action item.</param>
    /// <param name="style">The style of the action item.</param>
    /// <param name="description">The description of the action item.</param>
    /// <param name="url">The url of the action item.</param>
    /// <param name="emote">The emote of the action item.</param>
    /// <param name="isDisabled">Whether the action item is disabled.</param>
    /// <param name="id">The id of the action item.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuGroupBuilder WithMenuActionItem(string label, string customId = null,
        MenuActionItemStyle style = MenuActionItemStyle.Default, string description = null, string url = null,
        IEmote emote = null, bool isDisabled = false, int? id = null)
        => WithMenuActionItem(new MenuActionItemBuilder(label, customId, style, description, url, emote, isDisabled, id));

    /// <summary>
    ///     Adds an action item to this group.
    /// </summary>
    /// <param name="options">A delegate that configures the action item.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuGroupBuilder WithMenuActionItem(Action<MenuActionItemBuilder> options)
    {
        var component = new MenuActionItemBuilder();
        options(component);
        return WithMenuActionItem(component);
    }

    /// <summary>
    ///     Adds an action item to this group.
    /// </summary>
    /// <param name="component">The action item to add.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuGroupBuilder AddComponent(MenuActionItemBuilder component)
    {
        Components.Add(component);
        return this;
    }

    /// <summary>
    ///     Adds action items to this group.
    /// </summary>
    /// <param name="components">The action items to add.</param>
    /// <returns>The current builder.</returns>
    public OverflowMenuGroupBuilder AddComponents(params MenuActionItemBuilder[] components)
    {
        foreach (var component in components)
            Components.Add(component);
        return this;
    }

    /// <summary>
    ///     Builds this builder into an <see cref="OverflowMenuGroup"/>.
    /// </summary>
    /// <returns>An <see cref="OverflowMenuGroup"/>.</returns>
    /// <exception cref="ArgumentException">A group must contain between 1 and <see cref="MaxChildCount"/> action items.</exception>
    /// <exception cref="ArgumentException"><see cref="Label"/> exceeds <see cref="MaxLabelLength"/>.</exception>
    public OverflowMenuGroup Build()
    {
        Preconditions.NotNull(Components, nameof(Components));
        Preconditions.AtLeast(Components.Count, 1, nameof(Components), "An overflow menu group must contain at least 1 action item.");
        Preconditions.AtMost(Components.Count, MaxChildCount, nameof(Components), $"An overflow menu group can only contain {MaxChildCount} action items.");
        Preconditions.AtMost(Label?.Length ?? 0, MaxLabelLength, nameof(Label));

        return new OverflowMenuGroup(Label, Components.Select(x => x.Build()).ToImmutableArray());
    }
}
