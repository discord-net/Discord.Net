using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord;

/// <summary>
///     Represents a builder for creating a <see cref="MessageComponent"/>.
/// </summary>
public class ComponentBuilder
{
    /// <summary>
    ///     The max length of a <see cref="ButtonComponent.CustomId"/>.
    /// </summary>
    public const int MaxCustomIdLength = 100;

    /// <summary>
    ///     The max amount of rows a message can have.
    /// </summary>
    public const int MaxActionRowCount = 5;

    /// <summary>
    ///     Gets or sets the Action Rows for this Component Builder.
    /// </summary>
    /// <exception cref="ArgumentNullException" accessor="set"><see cref="ActionRows"/> cannot be null.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="ActionRows"/> count exceeds <see cref="MaxActionRowCount"/>.</exception>
    public List<ActionRowBuilder> ActionRows
    {
        get => _actionRows;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), $"{nameof(ActionRows)} cannot be null.");
            if (value.Count > MaxActionRowCount)
                throw new ArgumentOutOfRangeException(nameof(value), $"Action row count must be less than or equal to {MaxActionRowCount}.");
            _actionRows = value;
        }
    }

    private List<ActionRowBuilder> _actionRows;

    /// <summary>
    ///     Creates a new builder from a message.
    /// </summary>
    /// <param name="message">The message to create the builder from.</param>
    /// <returns>The newly created builder.</returns>
    public static ComponentBuilder FromMessage(IMessage message)
        => FromComponents(message.Components);

    /// <summary>
    ///     Creates a new builder from the provided list of components.
    /// </summary>
    /// <param name="components">The components to create the builder from.</param>
    /// <returns>The newly created builder.</returns>
    public static ComponentBuilder FromComponents(IReadOnlyCollection<IMessageComponent> components)
    {
        var builder = new ComponentBuilder();
        for (int i = 0; i != components.Count; i++)
        {
            var component = components.ElementAt(i);
            builder.AddComponent(component, i);
        }
        return builder;
    }

    internal void AddComponent(IMessageComponent component, int row)
    {
        switch (component)
        {
            case ButtonComponent button:
                WithButton(button.Label, button.CustomId, button.Style, button.Emote, button.Url, button.IsDisabled, row);
                break;
            case ActionRowComponent actionRow:
                foreach (var cmp in actionRow.Components)
                    AddComponent(cmp, row);
                break;
            case SelectMenuComponent menu:
                WithSelectMenu(menu.CustomId, menu.Options?.Select(x => new SelectMenuOptionBuilder(x.Label, x.Value, x.Description, x.Emote, x.IsDefault)).ToList(), menu.Placeholder, menu.MinValues, menu.MaxValues, menu.IsDisabled, row);
                break;
        }
    }

    /// <summary>
    ///     Removes all components of the given type from the <see cref="ComponentBuilder"/>.
    /// </summary>
    /// <param name="t">The <see cref="ComponentType"/> to remove.</param>
    /// <returns>The current builder.</returns>
    public ComponentBuilder RemoveComponentsOfType(ComponentType t)
    {
        this.ActionRows.ForEach(ar => ar.Components.RemoveAll(c => c.Type == t));
        return this;
    }

    /// <summary>
    ///     Removes a component from the <see cref="ComponentBuilder"/>.
    /// </summary>
    /// <param name="customId">The custom id of the component.</param>
    /// <returns>The current builder.</returns>
    public ComponentBuilder RemoveComponent(string customId)
    {
        this.ActionRows.ForEach(ar => ar.Components.RemoveAll(c => c.CustomId == customId));
        return this;
    }

    /// <summary>
    ///     Removes a Link Button from the <see cref="ComponentBuilder"/> based on its URL.
    /// </summary>
    /// <param name="url">The URL of the Link Button.</param>
    /// <returns>The current builder.</returns>
    public ComponentBuilder RemoveButtonByURL(string url)
    {
        this.ActionRows.ForEach(ar => ar.Components.RemoveAll(c => c is ButtonComponent b && b.Url == url));
        return this;
    }

    /// <summary>
    ///     Adds a <see cref="SelectMenuBuilder"/> to the <see cref="ComponentBuilder"/> at the specific row.
    ///     If the row cannot accept the component then it will add it to a row that can.
    /// </summary>
    /// <param name="customId">The custom id of the menu.</param>
    /// <param name="options">The options of the menu.</param>
    /// <param name="placeholder">The placeholder of the menu.</param>
    /// <param name="minValues">The min values of the placeholder.</param>
    /// <param name="maxValues">The max values of the placeholder.</param>
    /// <param name="disabled">Whether or not the menu is disabled.</param>
    /// <param name="row">The row to add the menu to.</param>
    /// <param name="type">The type of the select menu.</param>
    /// <param name="channelTypes">Menus valid channel types (only for <see cref="ComponentType.ChannelSelect"/>)</param>
    /// <returns></returns>
    public ComponentBuilder WithSelectMenu(string customId, List<SelectMenuOptionBuilder> options = null,
        string placeholder = null, int minValues = 1, int maxValues = 1, bool disabled = false, int row = 0, ComponentType type = ComponentType.SelectMenu,
        ChannelType[] channelTypes = null, SelectMenuDefaultValue[] defaultValues = null)
    {
        return WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId(customId)
                .WithOptions(options)
                .WithPlaceholder(placeholder)
                .WithMaxValues(maxValues)
                .WithMinValues(minValues)
                .WithDisabled(disabled)
                .WithType(type)
                .WithChannelTypes(channelTypes)
                .WithDefaultValues(defaultValues),
            row);
    }

    /// <summary>
    ///     Adds a <see cref="SelectMenuBuilder"/> to the <see cref="ComponentBuilder"/> at the specific row.
    ///     If the row cannot accept the component then it will add it to a row that can.
    /// </summary>
    /// <param name="menu">The menu to add.</param>
    /// <param name="row">The row to attempt to add this component on.</param>
    /// <exception cref="InvalidOperationException">There is no more row to add a menu.</exception>
    /// <exception cref="ArgumentException"><paramref name="row"/> must be less than <see cref="MaxActionRowCount"/>.</exception>
    /// <returns>The current builder.</returns>
    public ComponentBuilder WithSelectMenu(SelectMenuBuilder menu, int row = 0)
    {
        Preconditions.LessThan(row, MaxActionRowCount, nameof(row));
        if (menu.Options is not null && menu.Options.Distinct().Count() != menu.Options.Count)
            throw new InvalidOperationException("Please make sure that there is no duplicates values.");

        var builtMenu = menu.Build();

        if (_actionRows == null)
        {
            _actionRows = new List<ActionRowBuilder>
            {
                new ActionRowBuilder().AddComponent(builtMenu)
            };
        }
        else
        {
            if (_actionRows.Count == row)
                _actionRows.Add(new ActionRowBuilder().AddComponent(builtMenu));
            else
            {
                ActionRowBuilder actionRow;
                if (_actionRows.Count > row)
                    actionRow = _actionRows.ElementAt(row);
                else
                {
                    actionRow = new ActionRowBuilder();
                    _actionRows.Add(actionRow);
                }

                if (actionRow.CanTakeComponent(builtMenu))
                    actionRow.AddComponent(builtMenu);
                else if (row < MaxActionRowCount)
                    WithSelectMenu(menu, row + 1);
                else
                    throw new InvalidOperationException($"There is no more row to add a {nameof(builtMenu)}");
            }
        }

        return this;
    }

    /// <summary>
    ///     Adds a <see cref="ButtonBuilder"/> with specified parameters to the <see cref="ComponentBuilder"/> at the specific row.
    ///     If the row cannot accept the component then it will add it to a row that can.
    /// </summary>
    /// <param name="label">The label text for the newly added button.</param>
    /// <param name="style">The style of this newly added button.</param>
    /// <param name="emote">A <see cref="IEmote"/> to be used with this button.</param>
    /// <param name="customId">The custom id of the newly added button.</param>
    /// <param name="url">A URL to be used only if the <see cref="ButtonStyle"/> is a Link.</param>
    /// <param name="disabled">Whether or not the newly created button is disabled.</param>
    /// <param name="row">The row the button should be placed on.</param>
    /// <param name="skuId">The id of the sku associated with the current button.</param>
    /// <returns>The current builder.</returns>
    public ComponentBuilder WithButton(
        string label = null,
        string customId = null,
        ButtonStyle style = ButtonStyle.Primary,
        IEmote emote = null,
        string url = null,
        bool disabled = false,
        int row = 0,
        ulong? skuId = null)
    {
        var button = new ButtonBuilder()
            .WithLabel(label)
            .WithStyle(style)
            .WithEmote(emote)
            .WithCustomId(customId)
            .WithUrl(url)
            .WithDisabled(disabled)
            .WithSkuId(skuId);

        return WithButton(button, row);
    }

    /// <summary>
    ///     Adds a <see cref="ButtonBuilder"/> to the <see cref="ComponentBuilder"/> at the specific row.
    ///     If the row cannot accept the component then it will add it to a row that can.
    /// </summary>
    /// <param name="button">The button to add.</param>
    /// <param name="row">The row to add the button.</param>
    /// <exception cref="InvalidOperationException">There is no more row to add a button.</exception>
    /// <exception cref="ArgumentException"><paramref name="row"/> must be less than <see cref="MaxActionRowCount"/>.</exception>
    /// <returns>The current builder.</returns>
    public ComponentBuilder WithButton(ButtonBuilder button, int row = 0)
    {
        Preconditions.LessThan(row, MaxActionRowCount, nameof(row));

        var builtButton = button.Build();

        if (_actionRows == null)
        {
            _actionRows = new List<ActionRowBuilder>
            {
                new ActionRowBuilder().AddComponent(builtButton)
            };
        }
        else
        {
            if (_actionRows.Count == row)
                _actionRows.Add(new ActionRowBuilder().AddComponent(builtButton));
            else
            {
                ActionRowBuilder actionRow;
                if (_actionRows.Count > row)
                    actionRow = _actionRows.ElementAt(row);
                else
                {
                    actionRow = new ActionRowBuilder();
                    _actionRows.Add(actionRow);
                }

                if (actionRow.CanTakeComponent(builtButton))
                    actionRow.AddComponent(builtButton);
                else if (row < MaxActionRowCount)
                    WithButton(button, row + 1);
                else
                    throw new InvalidOperationException($"There is no more row to add a {nameof(button)}");
            }
        }

        return this;
    }

    /// <summary>
    ///     Adds a row to this component builder.
    /// </summary>
    /// <param name="row">The row to add.</param>
    /// <exception cref="IndexOutOfRangeException">The component builder contains the max amount of rows defined as <see cref="MaxActionRowCount"/>.</exception>
    /// <returns>The current builder.</returns>
    public ComponentBuilder AddRow(ActionRowBuilder row)
    {
        _actionRows ??= new();

        if (_actionRows.Count >= MaxActionRowCount)
            throw new IndexOutOfRangeException("The max amount of rows has been reached");

        ActionRows.Add(row);
        return this;
    }

    /// <summary>
    ///     Sets the rows of this component builder to a specified collection.
    /// </summary>
    /// <param name="rows">The rows to set.</param>
    /// <exception cref="IndexOutOfRangeException">The collection contains more rows then is allowed by discord.</exception>
    /// <returns>The current builder.</returns>
    public ComponentBuilder WithRows(IEnumerable<ActionRowBuilder> rows)
    {
        if (rows.Count() > MaxActionRowCount)
            throw new IndexOutOfRangeException($"Cannot have more than {MaxActionRowCount} rows");

        _actionRows = new List<ActionRowBuilder>(rows);
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="MessageComponent"/> used to send your components.
    /// </summary>
    /// <returns>A <see cref="MessageComponent"/> that can be sent with <see cref="IMessageChannel.SendMessageAsync"/>.</returns>
    public MessageComponent Build()
    {
        if (_actionRows?.SelectMany(x => x.Components)?.Any(x => x.Type == ComponentType.TextInput) ?? false)
            throw new ArgumentException("TextInputComponents are not allowed in messages.", nameof(ActionRows));

        if (_actionRows?.Count > 0)
            for (int i = 0; i < _actionRows?.Count; i++)
                if (_actionRows[i]?.Components?.Count == 0)
                    _actionRows.RemoveAt(i);

        return _actionRows != null
            ? new MessageComponent(_actionRows.Select(x => x.Build()).ToList())
            : MessageComponent.Empty;
    }
}
