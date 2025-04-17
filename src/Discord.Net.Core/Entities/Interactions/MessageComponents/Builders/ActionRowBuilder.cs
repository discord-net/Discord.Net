using Discord.Utils;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Discord;

/// <summary>
///     Represents a class used to build Action rows.
/// </summary>
public class ActionRowBuilder
{
    /// <summary>
    ///     The max amount of child components this row can hold.
    /// </summary>
    public const int MaxChildCount = 5;

    /// <summary>
    ///     Gets or sets the components inside this row.
    /// </summary>
    /// <exception cref="ArgumentNullException" accessor="set"><see cref="Components"/> cannot be null.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Components"/> count exceeds <see cref="MaxChildCount"/>.</exception>
    public List<IMessageComponent> Components
    {
        get => _components;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), $"{nameof(Components)} cannot be null.");

            _components = value.Count switch
            {
                0 => throw new ArgumentOutOfRangeException(nameof(value), "There must be at least 1 component in a row."),
                > MaxChildCount => throw new ArgumentOutOfRangeException(nameof(value), $"Action row can only contain {MaxChildCount} child components!"),
                _ => value
            };
        }
    }

    private List<IMessageComponent> _components = new List<IMessageComponent>();

    /// <summary>
    ///     Adds a list of components to the current row.
    /// </summary>
    /// <param name="components">The list of components to add.</param>
    /// <inheritdoc cref="Components"/>
    /// <returns>The current builder.</returns>
    public ActionRowBuilder WithComponents(List<IMessageComponent> components)
    {
        Components = components;
        return this;
    }

    /// <summary>
    ///     Adds a component at the end of the current row.
    /// </summary>
    /// <param name="component">The component to add.</param>
    /// <exception cref="InvalidOperationException">Components count reached <see cref="MaxChildCount"/></exception>
    /// <returns>The current builder.</returns>
    public ActionRowBuilder AddComponent(IMessageComponent component)
    {
        if (Components.Count >= MaxChildCount)
            throw new InvalidOperationException($"Components count reached {MaxChildCount}");

        Components.Add(component);
        return this;
    }

    /// <summary>
    ///     Adds a <see cref="SelectMenuBuilder"/> to the <see cref="ActionRowBuilder"/>.
    /// </summary>
    /// <param name="customId">The custom id of the menu.</param>
    /// <param name="options">The options of the menu.</param>
    /// <param name="placeholder">The placeholder of the menu.</param>
    /// <param name="minValues">The min values of the placeholder.</param>
    /// <param name="maxValues">The max values of the placeholder.</param>
    /// <param name="disabled">Whether or not the menu is disabled.</param> 
    /// <param name="type">The type of the select menu.</param>
    /// <param name="channelTypes">Menus valid channel types (only for <see cref="ComponentType.ChannelSelect"/>)</param>
    /// <returns>The current builder.</returns>
    public ActionRowBuilder WithSelectMenu(string customId, List<SelectMenuOptionBuilder> options = null,
        string placeholder = null, int minValues = 1, int maxValues = 1, bool disabled = false,
        ComponentType type = ComponentType.SelectMenu, ChannelType[] channelTypes = null)
    {
        return WithSelectMenu(new SelectMenuBuilder()
            .WithCustomId(customId)
            .WithOptions(options)
            .WithPlaceholder(placeholder)
            .WithMaxValues(maxValues)
            .WithMinValues(minValues)
            .WithDisabled(disabled)
            .WithType(type)
            .WithChannelTypes(channelTypes));
    }

    /// <summary>
    ///     Adds a <see cref="SelectMenuBuilder"/> to the <see cref="ActionRowBuilder"/>.
    /// </summary>
    /// <param name="menu">The menu to add.</param>
    /// <exception cref="InvalidOperationException">A Select Menu cannot exist in a pre-occupied ActionRow.</exception>
    /// <returns>The current builder.</returns>
    public ActionRowBuilder WithSelectMenu(SelectMenuBuilder menu)
    {
        if (menu.Options is not null && menu.Options.Distinct().Count() != menu.Options.Count)
            throw new InvalidOperationException("Please make sure that there is no duplicates values.");

        var builtMenu = menu.Build();

        if (Components.Count != 0)
            throw new InvalidOperationException($"A Select Menu cannot exist in a pre-occupied ActionRow.");

        AddComponent(builtMenu);

        return this;
    }

    /// <summary>
    ///     Adds a <see cref="ButtonBuilder"/> with specified parameters to the <see cref="ActionRowBuilder"/>.
    /// </summary>
    /// <param name="label">The label text for the newly added button.</param>
    /// <param name="style">The style of this newly added button.</param>
    /// <param name="emote">A <see cref="IEmote"/> to be used with this button.</param>
    /// <param name="customId">The custom id of the newly added button.</param>
    /// <param name="url">A URL to be used only if the <see cref="ButtonStyle"/> is a Link.</param>
    /// <param name="disabled">Whether or not the newly created button is disabled.</param>
    /// <returns>The current builder.</returns>
    public ActionRowBuilder WithButton(
        string label = null,
        string customId = null,
        ButtonStyle style = ButtonStyle.Primary,
        IEmote emote = null,
        string url = null,
        bool disabled = false)
    {
        var button = new ButtonBuilder()
            .WithLabel(label)
            .WithStyle(style)
            .WithEmote(emote)
            .WithCustomId(customId)
            .WithUrl(url)
            .WithDisabled(disabled);

        return WithButton(button);
    }

    /// <summary>
    ///     Adds a <see cref="ButtonBuilder"/> to the <see cref="ActionRowBuilder"/>.
    /// </summary>
    /// <param name="button">The button to add.</param>
    /// <exception cref="InvalidOperationException">Components count reached <see cref="MaxChildCount"/>.</exception>
    /// <exception cref="InvalidOperationException">A button cannot be added to a row with a SelectMenu.</exception>
    /// <returns>The current builder.</returns>
    public ActionRowBuilder WithButton(ButtonBuilder button)
    {
        var builtButton = button.Build();

        if (Components.Count >= 5)
            throw new InvalidOperationException($"Components count reached {MaxChildCount}");

        if (Components.Any(x => x.Type.IsSelectType()))
            throw new InvalidOperationException($"A button cannot be added to a row with a SelectMenu");

        AddComponent(builtButton);

        return this;
    }

    /// <summary>
    ///     Builds the current builder to a <see cref="ActionRowComponent"/> that can be used within a <see cref="ComponentBuilder"/>
    /// </summary>
    /// <returns>A <see cref="ActionRowComponent"/> that can be used within a <see cref="ComponentBuilder"/></returns>
    public ActionRowComponent Build()
    {
        return new ActionRowComponent(_components);
    }

    internal bool CanTakeComponent(IMessageComponent component)
    {
        switch (component.Type)
        {
            case ComponentType.ActionRow:
                return false;
            case ComponentType.Button:
                if (Components.Any(x => x.Type.IsSelectType()))
                    return false;
                else
                    return Components.Count < 5;
            case ComponentType.SelectMenu:
            case ComponentType.ChannelSelect:
            case ComponentType.MentionableSelect:
            case ComponentType.RoleSelect:
            case ComponentType.UserSelect:
                return Components.Count == 0;
            default:
                return false;
        }
    }
}
