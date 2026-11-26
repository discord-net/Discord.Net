using Discord.Utils;

using System;

namespace Discord;

/// <summary>
///     Represents a class used to build <see cref="MenuActionItemComponent"/>'s.
/// </summary>
public class MenuActionItemBuilder : IInteractableComponentBuilder
{
    /// <summary>
    ///     The maximum length of the label.
    /// </summary>
    public const int MaxLabelLength = 80;

    /// <summary>
    ///     The maximum length of the description.
    /// </summary>
    public const int MaxDescriptionLength = 100;

    /// <inheritdoc/>
    public ComponentType Type => ComponentType.MenuActionItem;

    /// <inheritdoc/>
    public int? Id { get; set; }

    /// <summary>
    ///     Gets or sets the custom id of the current action item.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length exceeds <see cref="ComponentBuilder.MaxCustomIdLength"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length subceeds 1.</exception>
    public string CustomId
    {
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtLeast(value.Length, 1, nameof(CustomId));
                Preconditions.AtMost(value.Length, ComponentBuilder.MaxCustomIdLength, nameof(CustomId));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets the label of the current action item.
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
    ///     Gets or sets the <see cref="MenuActionItemStyle"/> of the current action item.
    /// </summary>
    public MenuActionItemStyle Style { get; set; } = MenuActionItemStyle.Default;

    /// <summary>
    ///     Gets or sets the description of the current action item.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Description"/> length exceeds <see cref="MaxDescriptionLength"/>.</exception>
    public string Description
    {
        get;
        set
        {
            if (value is not null)
            {
                Preconditions.AtMost(value.Length, MaxDescriptionLength, nameof(Description));
            }

            field = value;
        }
    }

    /// <summary>
    ///     Gets or sets the url of the current action item.
    /// </summary>
    /// <remarks>
    ///     Only used when <see cref="Style"/> is <see cref="MenuActionItemStyle.Link"/>.
    /// </remarks>
    public string Url { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="IEmote"/> of the current action item.
    /// </summary>
    public IEmote Emote { get; set; }

    /// <summary>
    ///     Gets or sets whether the current action item is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MenuActionItemBuilder"/>.
    /// </summary>
    public MenuActionItemBuilder() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MenuActionItemBuilder"/>.
    /// </summary>
    /// <param name="label">The label of the action item.</param>
    /// <param name="customId">The custom id of the action item.</param>
    /// <param name="style">The style of the action item.</param>
    /// <param name="description">The description of the action item.</param>
    /// <param name="url">The url of the action item.</param>
    /// <param name="emote">The emote of the action item.</param>
    /// <param name="isDisabled">Whether the action item is disabled.</param>
    /// <param name="id">The id of the action item.</param>
    public MenuActionItemBuilder(string label = null, string customId = null,
        MenuActionItemStyle style = MenuActionItemStyle.Default, string description = null, string url = null,
        IEmote emote = null, bool isDisabled = false, int? id = null)
    {
        Label = label;
        CustomId = customId;
        Style = style;
        Description = description;
        Url = url;
        Emote = emote;
        IsDisabled = isDisabled;
        Id = id;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MenuActionItemBuilder"/> class from an existing <see cref="MenuActionItemComponent"/>.
    /// </summary>
    /// <param name="actionItem">The component.</param>
    public MenuActionItemBuilder(MenuActionItemComponent actionItem)
    {
        Label = actionItem.Label;
        CustomId = actionItem.CustomId;
        Style = actionItem.Style;
        Description = actionItem.Description;
        Url = actionItem.Url;
        Emote = actionItem.Emote;
        IsDisabled = actionItem.IsDisabled;
        Id = actionItem.Id;
    }

    /// <summary>
    ///     Creates an action item with the <see cref="MenuActionItemStyle.Link"/> style.
    /// </summary>
    /// <param name="label">The label for this link action item.</param>
    /// <param name="url">The url for this link action item to go to.</param>
    /// <param name="emote">The emote for this link action item.</param>
    /// <returns>A builder with the newly created action item.</returns>
    public static MenuActionItemBuilder CreateLinkItem(string label, string url, IEmote emote = null)
        => new(label, null, MenuActionItemStyle.Link, url: url, emote: emote);

    /// <summary>
    ///     Sets the label of the current action item.
    /// </summary>
    /// <param name="label">The label to use for the current action item.</param>
    /// <returns>The current builder.</returns>
    public MenuActionItemBuilder WithLabel(string label)
    {
        Label = label;
        return this;
    }

    /// <summary>
    ///     Sets the custom id of the current action item.
    /// </summary>
    /// <param name="customId">The id to use for the current action item.</param>
    /// <inheritdoc cref="CustomId"/>
    /// <returns>The current builder.</returns>
    public MenuActionItemBuilder WithCustomId(string customId)
    {
        CustomId = customId;
        return this;
    }

    /// <summary>
    ///     Sets the style of the current action item.
    /// </summary>
    /// <param name="style">The style to use for the current action item.</param>
    /// <returns>The current builder.</returns>
    public MenuActionItemBuilder WithStyle(MenuActionItemStyle style)
    {
        Style = style;
        return this;
    }

    /// <summary>
    ///     Sets the description of the current action item.
    /// </summary>
    /// <param name="description">The description to use for the current action item.</param>
    /// <returns>The current builder.</returns>
    public MenuActionItemBuilder WithDescription(string description)
    {
        Description = description;
        return this;
    }

    /// <summary>
    ///     Sets the url of the current action item.
    /// </summary>
    /// <param name="url">The url to use for the current action item.</param>
    /// <returns>The current builder.</returns>
    public MenuActionItemBuilder WithUrl(string url)
    {
        Url = url;
        return this;
    }

    /// <summary>
    ///     Sets the emote of the current action item.
    /// </summary>
    /// <param name="emote">The emote to use for the current action item.</param>
    /// <returns>The current builder.</returns>
    public MenuActionItemBuilder WithEmote(IEmote emote)
    {
        Emote = emote;
        return this;
    }

    /// <summary>
    ///     Sets whether the current action item is disabled.
    /// </summary>
    /// <param name="isDisabled">Whether the current action item is disabled or not.</param>
    /// <returns>The current builder.</returns>
    public MenuActionItemBuilder WithDisabled(bool isDisabled)
    {
        IsDisabled = isDisabled;
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="MenuActionItemComponent"/>.
    /// </summary>
    /// <returns>A <see cref="MenuActionItemComponent"/>.</returns>
    /// <exception cref="ArgumentNullException">A menu action item must have a label.</exception>
    /// <exception cref="ArgumentException"><see cref="Label"/> or <see cref="Description"/> exceeds the maximum length.</exception>
    /// <exception cref="InvalidOperationException">A non-link action item must contain a <see cref="CustomId"/>.</exception>
    /// <exception cref="InvalidOperationException">A link action item must contain a <see cref="Url"/>.</exception>
    /// <exception cref="InvalidOperationException">An action item cannot contain both a <see cref="Url"/> and a <see cref="CustomId"/>.</exception>
    public MenuActionItemComponent Build()
    {
        Preconditions.NotNullOrWhitespace(Label, nameof(Label));
        Preconditions.AtMost(Label.Length, MaxLabelLength, nameof(Label));
        Preconditions.AtMost(Description?.Length ?? 0, MaxDescriptionLength, nameof(Description));

        switch (Style)
        {
            case MenuActionItemStyle.Default:
            case MenuActionItemStyle.Danger:
            {
                if (string.IsNullOrWhiteSpace(CustomId))
                    throw new InvalidOperationException("Non-link menu action items must have a custom id associated with them.");

                if (!string.IsNullOrWhiteSpace(Url))
                    throw new InvalidOperationException("Only link menu action items can have a URL associated with them.");
            }
            break;

            case MenuActionItemStyle.Link:
            {
                if (string.IsNullOrWhiteSpace(Url))
                    throw new InvalidOperationException("Link menu action items must have a URL associated with them.");

                if (!string.IsNullOrWhiteSpace(CustomId))
                    throw new InvalidOperationException("Link menu action items cannot have a custom id associated with them.");

                UrlValidation.ValidateButton(Url);
            }
            break;

            default:
                throw new ArgumentException("A menu action item must have a style.", nameof(Style));
        }

        return new MenuActionItemComponent(Id, CustomId, Label, Style, Description, Url, Emote, IsDisabled);
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
