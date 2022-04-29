using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Utils;

namespace Discord
{
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
                    WithSelectMenu(menu.CustomId, menu.Options.Select(x => new SelectMenuOptionBuilder(x.Label, x.Value, x.Description, x.Emote, x.IsDefault)).ToList(), menu.Placeholder, menu.MinValues, menu.MaxValues, menu.IsDisabled, row);
                    break;
            }
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
        /// <returns></returns>
        public ComponentBuilder WithSelectMenu(string customId, List<SelectMenuOptionBuilder> options,
            string placeholder = null, int minValues = 1, int maxValues = 1, bool disabled = false, int row = 0)
        {
            return WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId(customId)
                .WithOptions(options)
                .WithPlaceholder(placeholder)
                .WithMaxValues(maxValues)
                .WithMinValues(minValues)
                .WithDisabled(disabled),
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
            if (menu.Options.Distinct().Count() != menu.Options.Count)
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
        /// <returns>The current builder.</returns>
        public ComponentBuilder WithButton(
            string label = null,
            string customId = null,
            ButtonStyle style = ButtonStyle.Primary,
            IEmote emote = null,
            string url = null,
            bool disabled = false,
            int row = 0)
        {
            var button = new ButtonBuilder()
                .WithLabel(label)
                .WithStyle(style)
                .WithEmote(emote)
                .WithCustomId(customId)
                .WithUrl(url)
                .WithDisabled(disabled);

            return WithButton(button, row);
        }

        /// <summary>
        ///     Adds a <see cref="ButtonBuilder"/> to the <see cref="ComponentBuilder"/> at the specific row.
        ///     If the row cannot accept the component then it will add it to a row that can.
        /// </summary>
        /// <param name="button">The button to add.</param>
        /// <param name="row">The row to add the button.</param>
        /// <exception cref="InvalidOperationException">There is no more row to add a menu.</exception>
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
            if (_actionRows?.SelectMany(x => x.Components)?.Any(x => x.Type == ComponentType.ModalSubmit) ?? false)
                throw new ArgumentException("ModalSubmit components are not allowed in messages.", nameof(ActionRows));
            
            return _actionRows != null
                ? new MessageComponent(_actionRows.Select(x => x.Build()).ToList())
                : MessageComponent.Empty;
        }
    }

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
                    if (Components.Any(x => x.Type == ComponentType.SelectMenu))
                        return false;
                    else
                        return Components.Count < 5;
                case ComponentType.SelectMenu:
                    return Components.Count == 0;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    ///     Represents a class used to build <see cref="ButtonComponent"/>'s.
    /// </summary>
    public class ButtonBuilder
    {
        /// <summary>
        ///     The max length of a <see cref="ButtonComponent.Label"/>.
        /// </summary>
        public const int MaxButtonLabelLength = 80;

        /// <summary>
        ///     Gets or sets the label of the current button.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Label"/> length exceeds <see cref="MaxButtonLabelLength"/>.</exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Label"/> length exceeds <see cref="MaxButtonLabelLength"/>.</exception>
        public string Label
        {
            get => _label;
            set => _label = value?.Length switch
            {
                > MaxButtonLabelLength => throw new ArgumentOutOfRangeException(nameof(value), $"Label length must be less or equal to {MaxButtonLabelLength}."),
                0 => throw new ArgumentOutOfRangeException(nameof(value), "Label length must be at least 1."),
                _ => value
            };
        }

        /// <summary>
        ///     Gets or sets the custom id of the current button.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length exceeds <see cref="ComponentBuilder.MaxCustomIdLength"/></exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length subceeds 1.</exception>
        public string CustomId
        {
            get => _customId;
            set => _customId = value?.Length switch
            {
                > ComponentBuilder.MaxCustomIdLength => throw new ArgumentOutOfRangeException(nameof(value), $"Custom Id length must be less or equal to {ComponentBuilder.MaxCustomIdLength}."),
                0 => throw new ArgumentOutOfRangeException(nameof(value), "Custom Id length must be at least 1."),
                _ => value
            };
        }

        /// <summary>
        ///     Gets or sets the <see cref="ButtonStyle"/> of the current button.
        /// </summary>
        public ButtonStyle Style { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="IEmote"/> of the current button.
        /// </summary>
        public IEmote Emote { get; set; }

        /// <summary>
        ///     Gets or sets the url of the current button.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     Gets or sets whether the current button is disabled.
        /// </summary>
        public bool IsDisabled { get; set; }

        private string _label;
        private string _customId;

        /// <summary>
        ///     Creates a new instance of a <see cref="ButtonBuilder"/>.
        /// </summary>
        public ButtonBuilder() { }

        /// <summary>
        ///     Creates a new instance of a <see cref="ButtonBuilder"/>.
        /// </summary>
        /// <param name="label">The label to use on the newly created link button.</param>
        /// <param name="url">The url of this button.</param>
        /// <param name="customId">The custom ID of this button.</param>
        /// <param name="style">The custom ID of this button.</param>
        /// <param name="emote">The emote of this button.</param>
        /// <param name="isDisabled">Disabled this button or not.</param>
        public ButtonBuilder(string label = null, string customId = null, ButtonStyle style = ButtonStyle.Primary, string url = null, IEmote emote = null, bool isDisabled = false)
        {
            CustomId = customId;
            Style = style;
            Url = url;
            Label = label;
            IsDisabled = isDisabled;
            Emote = emote;
        }

        /// <summary>
        ///     Creates a new instance of a <see cref="ButtonBuilder"/> from instance of a <see cref="ButtonComponent"/>.
        /// </summary>
        public ButtonBuilder(ButtonComponent button)
        {
            CustomId = button.CustomId;
            Style = button.Style;
            Url = button.Url;
            Label = button.Label;
            IsDisabled = button.IsDisabled;
            Emote = button.Emote;
        }

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Link"/> style.
        /// </summary>
        /// <param name="label">The label for this link button.</param>
        /// <param name="url">The url for this link button to go to.</param>
        /// <param name="emote">The emote for this link button.</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateLinkButton(string label, string url, IEmote emote = null)
            => new ButtonBuilder(label, null, ButtonStyle.Link, url, emote: emote);

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Danger"/> style.
        /// </summary>
        /// <param name="label">The label for this danger button.</param>
        /// <param name="customId">The custom id for this danger button.</param>
        /// <param name="emote">The emote for this danger button.</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateDangerButton(string label, string customId, IEmote emote = null)
            => new ButtonBuilder(label, customId, ButtonStyle.Danger, emote: emote);

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Primary"/> style.
        /// </summary>
        /// <param name="label">The label for this primary button.</param>
        /// <param name="customId">The custom id for this primary button.</param>
        /// <param name="emote">The emote for this primary button.</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreatePrimaryButton(string label, string customId, IEmote emote = null)
            => new ButtonBuilder(label, customId, emote: emote);

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Secondary"/> style.
        /// </summary>
        /// <param name="label">The label for this secondary button.</param>
        /// <param name="customId">The custom id for this secondary button.</param>
        /// <param name="emote">The emote for this secondary button.</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateSecondaryButton(string label, string customId, IEmote emote = null)
            => new ButtonBuilder(label, customId, ButtonStyle.Secondary, emote: emote);

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Success"/> style.
        /// </summary>
        /// <param name="label">The label for this success button.</param>
        /// <param name="customId">The custom id for this success button.</param>
        /// <param name="emote">The emote for this success button.</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateSuccessButton(string label, string customId, IEmote emote = null)
            => new ButtonBuilder(label, customId, ButtonStyle.Success, emote: emote);

        /// <summary>
        ///     Sets the current buttons label to the specified text.
        /// </summary>
        /// <param name="label">The text for the label.</param>
        /// <inheritdoc cref="Label"/>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithLabel(string label)
        {
            Label = label;
            return this;
        }

        /// <summary>
        ///     Sets the current buttons style.
        /// </summary>
        /// <param name="style">The style for this builders button.</param>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithStyle(ButtonStyle style)
        {
            Style = style;
            return this;
        }

        /// <summary>
        ///     Sets the current buttons emote.
        /// </summary>
        /// <param name="emote">The emote to use for the current button.</param>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithEmote(IEmote emote)
        {
            Emote = emote;
            return this;
        }

        /// <summary>
        ///     Sets the current buttons url.
        /// </summary>
        /// <param name="url">The url to use for the current button.</param>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithUrl(string url)
        {
            Url = url;
            return this;
        }

        /// <summary>
        ///     Sets the custom id of the current button.
        /// </summary>
        /// <param name="id">The id to use for the current button.</param>
        /// <inheritdoc cref="CustomId"/>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithCustomId(string id)
        {
            CustomId = id;
            return this;
        }

        /// <summary>
        ///     Sets whether the current button is disabled.
        /// </summary>
        /// <param name="isDisabled">Whether the current button is disabled or not.</param>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithDisabled(bool isDisabled)
        {
            IsDisabled = isDisabled;
            return this;
        }

        /// <summary>
        ///     Builds this builder into a <see cref="ButtonComponent"/> to be used in a <see cref="ComponentBuilder"/>.
        /// </summary>
        /// <returns>A <see cref="ButtonComponent"/> to be used in a <see cref="ComponentBuilder"/>.</returns>
        /// <exception cref="InvalidOperationException">A button must contain either a <see cref="Url"/> or a <see cref="CustomId"/>, but not both.</exception>
        /// <exception cref="InvalidOperationException">A button must have an <see cref="Emote"/> or a <see cref="Label"/>.</exception>
        /// <exception cref="InvalidOperationException">A link button must contain a URL.</exception>
        /// <exception cref="InvalidOperationException">A URL must include a protocol (http or https).</exception>
        /// <exception cref="InvalidOperationException">A non-link button must contain a custom id</exception>
        public ButtonComponent Build()
        {
            if (string.IsNullOrEmpty(Label) && Emote == null)
                throw new InvalidOperationException("A button must have an Emote or a label!");

            if (!(string.IsNullOrEmpty(Url) ^ string.IsNullOrEmpty(CustomId)))
                throw new InvalidOperationException("A button must contain either a URL or a CustomId, but not both!");

            if (Style == 0)
                throw new ArgumentException("A button must have a style.", nameof(Style));

            if (Style == ButtonStyle.Link)
            {
                if (string.IsNullOrEmpty(Url))
                    throw new InvalidOperationException("Link buttons must have a link associated with them");
                UrlValidation.ValidateButton(Url);
            }
            else if (string.IsNullOrEmpty(CustomId))
                throw new InvalidOperationException("Non-link buttons must have a custom id associated with them");

            return new ButtonComponent(Style, Label, Emote, CustomId, Url, IsDisabled);
        }
    }

    /// <summary>
    ///     Represents a class used to build <see cref="SelectMenuComponent"/>'s.
    /// </summary>
    public class SelectMenuBuilder
    {
        /// <summary>
        ///     The max length of a <see cref="SelectMenuComponent.Placeholder"/>.
        /// </summary>
        public const int MaxPlaceholderLength = 100;

        /// <summary>
        ///     The maximum number of values for the <see cref="SelectMenuComponent.MinValues"/> and <see cref="SelectMenuComponent.MaxValues"/> properties.
        /// </summary>
        public const int MaxValuesCount = 25;

        /// <summary>
        ///     The maximum number of options a <see cref="SelectMenuComponent"/> can have.
        /// </summary>
        public const int MaxOptionCount = 25;

        /// <summary>
        ///     Gets or sets the custom id of the current select menu.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length exceeds <see cref="ComponentBuilder.MaxCustomIdLength"/></exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length subceeds 1.</exception>
        public string CustomId
        {
            get => _customId;
            set => _customId = value?.Length switch
            {
                > ComponentBuilder.MaxCustomIdLength => throw new ArgumentOutOfRangeException(nameof(value), $"Custom Id length must be less or equal to {ComponentBuilder.MaxCustomIdLength}."),
                0 => throw new ArgumentOutOfRangeException(nameof(value), "Custom Id length must be at least 1."),
                _ => value
            };
        }

        /// <summary>
        ///     Gets or sets the placeholder text of the current select menu.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Placeholder"/> length exceeds <see cref="MaxPlaceholderLength"/>.</exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Placeholder"/> length subceeds 1.</exception>
        public string Placeholder
        {
            get => _placeholder;
            set => _placeholder = value?.Length switch
            {
                > MaxPlaceholderLength => throw new ArgumentOutOfRangeException(nameof(value), $"Placeholder length must be less or equal to {MaxPlaceholderLength}."),
                0 => throw new ArgumentOutOfRangeException(nameof(value), "Placeholder length must be at least 1."),
                _ => value
            };
        }

        /// <summary>
        ///     Gets or sets the minimum values of the current select menu.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="MinValues"/> exceeds <see cref="MaxValuesCount"/>.</exception>
        public int MinValues
        {
            get => _minValues;
            set
            {
                Preconditions.AtMost(value, MaxValuesCount, nameof(MinValues));
                _minValues = value;
            }
        }

        /// <summary>
        ///     Gets or sets the maximum values of the current select menu.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="MaxValues"/> exceeds <see cref="MaxValuesCount"/>.</exception>
        public int MaxValues
        {
            get => _maxValues;
            set
            {
                Preconditions.AtMost(value, MaxValuesCount, nameof(MaxValues));
                _maxValues = value;
            }
        }

        /// <summary>
        ///     Gets or sets a collection of <see cref="SelectMenuOptionBuilder"/> for this current select menu.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Options"/> count exceeds <see cref="MaxOptionCount"/>.</exception>
        /// <exception cref="ArgumentNullException" accessor="set"><see cref="Options"/> is null.</exception>
        public List<SelectMenuOptionBuilder> Options
        {
            get => _options;
            set
            {
                if (value != null)
                    Preconditions.AtMost(value.Count, MaxOptionCount, nameof(Options));
                else
                    throw new ArgumentNullException(nameof(value), $"{nameof(Options)} cannot be null.");

                _options = value;
            }
        }

        /// <summary>
        ///     Gets or sets whether the current menu is disabled.
        /// </summary>
        public bool IsDisabled { get; set; }

        private List<SelectMenuOptionBuilder> _options = new List<SelectMenuOptionBuilder>();
        private int _minValues = 1;
        private int _maxValues = 1;
        private string _placeholder;
        private string _customId;

        /// <summary>
        ///     Creates a new instance of a <see cref="SelectMenuBuilder"/>.
        /// </summary>
        public SelectMenuBuilder() { }

        /// <summary>
        ///     Creates a new instance of a <see cref="SelectMenuBuilder"/> from instance of <see cref="SelectMenuComponent"/>.
        /// </summary>
        public SelectMenuBuilder(SelectMenuComponent selectMenu)
        {
            Placeholder = selectMenu.Placeholder;
            CustomId = selectMenu.Placeholder;
            MaxValues = selectMenu.MaxValues;
            MinValues = selectMenu.MinValues;
            IsDisabled = selectMenu.IsDisabled;
            Options = selectMenu.Options?
               .Select(x => new SelectMenuOptionBuilder(x.Label, x.Value, x.Description, x.Emote, x.IsDefault))
               .ToList();
        }

        /// <summary>
        ///     Creates a new instance of a <see cref="SelectMenuBuilder"/>.
        /// </summary>
        /// <param name="customId">The custom id of this select menu.</param>
        /// <param name="options">The options for this select menu.</param>
        /// <param name="placeholder">The placeholder of this select menu.</param>
        /// <param name="maxValues">The max values of this select menu.</param>
        /// <param name="minValues">The min values of this select menu.</param>
        /// <param name="isDisabled">Disabled this select menu or not.</param>
        public SelectMenuBuilder(string customId, List<SelectMenuOptionBuilder> options, string placeholder = null, int maxValues = 1, int minValues = 1, bool isDisabled = false)
        {
            CustomId = customId;
            Options = options;
            Placeholder = placeholder;
            IsDisabled = isDisabled;
            MaxValues = maxValues;
            MinValues = minValues;
        }

        /// <summary>
        ///     Sets the field CustomId.
        /// </summary>
        /// <param name="customId">The value to set the field CustomId to.</param>
        /// <inheritdoc cref="CustomId"/>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder WithCustomId(string customId)
        {
            CustomId = customId;
            return this;
        }

        /// <summary>
        ///     Sets the field placeholder.
        /// </summary>
        /// <param name="placeholder">The value to set the field placeholder to.</param>
        /// <inheritdoc cref="Placeholder"/>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder WithPlaceholder(string placeholder)
        {
            Placeholder = placeholder;
            return this;
        }

        /// <summary>
        ///     Sets the field minValues.
        /// </summary>
        /// <param name="minValues">The value to set the field minValues to.</param>
        /// <inheritdoc cref="MinValues"/>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder WithMinValues(int minValues)
        {
            MinValues = minValues;
            return this;
        }

        /// <summary>
        ///     Sets the field maxValues.
        /// </summary>
        /// <param name="maxValues">The value to set the field maxValues to.</param>
        /// <inheritdoc cref="MaxValues"/>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder WithMaxValues(int maxValues)
        {
            MaxValues = maxValues;
            return this;
        }

        /// <summary>
        ///     Sets the field options.
        /// </summary>
        /// <param name="options">The value to set the field options to.</param>
        /// <inheritdoc cref="Options"/>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder WithOptions(List<SelectMenuOptionBuilder> options)
        {
            Options = options;
            return this;
        }

        /// <summary>
        ///     Add one option to menu options.
        /// </summary>
        /// <param name="option">The option builder class containing the option properties.</param>
        /// <exception cref="InvalidOperationException">Options count reached <see cref="MaxOptionCount"/>.</exception>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder AddOption(SelectMenuOptionBuilder option)
        {
            if (Options.Count >= MaxOptionCount)
                throw new InvalidOperationException($"Options count reached {MaxOptionCount}.");

            Options.Add(option);
            return this;
        }

        /// <summary>
        ///     Add one option to menu options.
        /// </summary>
        /// <param name="label">The label for this option.</param>
        /// <param name="value">The value of this option.</param>
        /// <param name="description">The description of this option.</param>
        /// <param name="emote">The emote of this option.</param>
        /// <param name="isDefault">Render this option as selected by default or not.</param>
        /// <exception cref="InvalidOperationException">Options count reached <see cref="MaxOptionCount"/>.</exception>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder AddOption(string label, string value, string description = null, IEmote emote = null, bool? isDefault = null)
        {
            AddOption(new SelectMenuOptionBuilder(label, value, description, emote, isDefault));
            return this;
        }

        /// <summary>
        ///     Sets whether the current menu is disabled.
        /// </summary>
        /// <param name="isDisabled">Whether the current menu is disabled or not.</param>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder WithDisabled(bool isDisabled)
        {
            IsDisabled = isDisabled;
            return this;
        }

        /// <summary>
        ///     Builds a <see cref="SelectMenuComponent"/>
        /// </summary>
        /// <returns>The newly built <see cref="SelectMenuComponent"/></returns>
        public SelectMenuComponent Build()
        {
            var options = Options?.Select(x => x.Build()).ToList();

            return new SelectMenuComponent(CustomId, options, Placeholder, MinValues, MaxValues, IsDisabled);
        }
    }

    /// <summary>
    ///     Represents a class used to build <see cref="SelectMenuOption"/>'s.
    /// </summary>
    public class SelectMenuOptionBuilder
    {
        /// <summary>
        ///     The maximum length of a <see cref="SelectMenuOption.Label"/>.
        /// </summary>
        public const int MaxSelectLabelLength = 100;

        /// <summary>
        ///     The maximum length of a <see cref="SelectMenuOption.Description"/>.
        /// </summary>
        public const int MaxDescriptionLength = 100;

        /// <summary>
        ///     The maximum length of a <see cref="SelectMenuOption.Value"/>.
        /// </summary>
        public const int MaxSelectValueLength = 100;

        /// <summary>
        ///     Gets or sets the label of the current select menu.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Label"/> length exceeds <see cref="MaxSelectLabelLength"/></exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Label"/> length subceeds 1.</exception>
        public string Label
        {
            get => _label;
            set => _label = value?.Length switch
            {
                > MaxSelectLabelLength => throw new ArgumentOutOfRangeException(nameof(value), $"Label length must be less or equal to {MaxSelectLabelLength}."),
                0 => throw new ArgumentOutOfRangeException(nameof(value), "Label length must be at least 1."),
                _ => value
            };
        }

        /// <summary>
        ///     Gets or sets the value of the current select menu.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Value"/> length exceeds <see cref="MaxSelectValueLength"/>.</exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Value"/> length subceeds 1.</exception>
        public string Value
        {
            get => _value;
            set => _value = value?.Length switch
            {
                > MaxSelectValueLength => throw new ArgumentOutOfRangeException(nameof(value), $"Value length must be less or equal to {MaxSelectValueLength}."),
                0 => throw new ArgumentOutOfRangeException(nameof(value), "Value length must be at least 1."),
                _ => value
            };
        }

        /// <summary>
        ///     Gets or sets this menu options description.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Description"/> length exceeds <see cref="MaxDescriptionLength"/>.</exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Description"/> length subceeds 1.</exception>
        public string Description
        {
            get => _description;
            set => _description = value?.Length switch
            {
                > MaxDescriptionLength => throw new ArgumentOutOfRangeException(nameof(value), $"Description length must be less or equal to {MaxDescriptionLength}."),
                0 => throw new ArgumentOutOfRangeException(nameof(value), "Description length must be at least 1."),
                _ => value
            };
        }

        /// <summary>
        ///     Gets or sets the emote of this option.
        /// </summary>
        public IEmote Emote { get; set; }

        /// <summary>
        ///     Gets or sets the whether or not this option will render selected by default.
        /// </summary>
        public bool? IsDefault { get; set; }

        private string _label;
        private string _value;
        private string _description;

        /// <summary>
        ///     Creates a new instance of a <see cref="SelectMenuOptionBuilder"/>.
        /// </summary>
        public SelectMenuOptionBuilder() { }

        /// <summary>
        ///     Creates a new instance of a <see cref="SelectMenuOptionBuilder"/>.
        /// </summary>
        /// <param name="label">The label for this option.</param>
        /// <param name="value">The value of this option.</param>
        /// <param name="description">The description of this option.</param>
        /// <param name="emote">The emote of this option.</param>
        /// <param name="isDefault">Render this option as selected by default or not.</param>
        public SelectMenuOptionBuilder(string label, string value, string description = null, IEmote emote = null, bool? isDefault = null)
        {
            Label = label;
            Value = value;
            Description = description;
            Emote = emote;
            IsDefault = isDefault;
        }

        /// <summary>
        ///     Creates a new instance of a <see cref="SelectMenuOptionBuilder"/> from instance of a <see cref="SelectMenuOption"/>.
        /// </summary>
        public SelectMenuOptionBuilder(SelectMenuOption option)
        {
            Label = option.Label;
            Value = option.Value;
            Description = option.Description;
            Emote = option.Emote;
            IsDefault = option.IsDefault;
        }

        /// <summary>
        ///     Sets the field label.
        /// </summary>
        /// <param name="label">The value to set the field label to.</param>
        /// <inheritdoc cref="Label"/>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuOptionBuilder WithLabel(string label)
        {
            Label = label;
            return this;
        }

        /// <summary>
        ///     Sets the field value.
        /// </summary>
        /// <param name="value">The value to set the field value to.</param>
        /// <inheritdoc cref="Value"/>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuOptionBuilder WithValue(string value)
        {
            Value = value;
            return this;
        }

        /// <summary>
        ///     Sets the field description.
        /// </summary>
        /// <param name="description">The value to set the field description to.</param>
        /// <inheritdoc cref="Description"/>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuOptionBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets the field emote.
        /// </summary>
        /// <param name="emote">The value to set the field emote to.</param>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuOptionBuilder WithEmote(IEmote emote)
        {
            Emote = emote;
            return this;
        }

        /// <summary>
        ///     Sets the field default.
        /// </summary>
        /// <param name="isDefault">The value to set the field default to.</param>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuOptionBuilder WithDefault(bool isDefault)
        {
            IsDefault = isDefault;
            return this;
        }

        /// <summary>
        ///     Builds a <see cref="SelectMenuOption"/>.
        /// </summary>
        /// <returns>The newly built <see cref="SelectMenuOption"/>.</returns>
        public SelectMenuOption Build()
        {
            return new SelectMenuOption(Label, Value, Description, Emote, IsDefault);
        }
    }

    public class TextInputBuilder
    {
        public const int LargestMaxLength = 4000;

        /// <summary>
        ///     Gets or sets the custom id of the current text input.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length exceeds <see cref="ComponentBuilder.MaxCustomIdLength"/></exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length subceeds 1.</exception>
        public string CustomId
        {
            get => _customId;
            set => _customId = value?.Length switch
            {
                > ComponentBuilder.MaxCustomIdLength => throw new ArgumentOutOfRangeException(nameof(value), $"Custom Id length must be less or equal to {ComponentBuilder.MaxCustomIdLength}."),
                0 => throw new ArgumentOutOfRangeException(nameof(value), "Custom Id length must be at least 1."),
                _ => value
            };
        }

        /// <summary>
        ///     Gets or sets the style of the current text input.
        /// </summary>
        public TextInputStyle Style { get; set; } = TextInputStyle.Short;

        /// <summary>
        ///     Gets or sets the label of the current text input.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     Gets or sets the placeholder of the current text input.
        /// </summary>
        /// <exception cref="ArgumentException"><see cref="Placeholder"/> is longer than 100 characters</exception>
        public string Placeholder
        {
            get => _placeholder;
            set => _placeholder = (value?.Length ?? 0) <= 100
                ? value
                : throw new ArgumentException("Placeholder cannot have more than 100 characters.");
        }

        /// <summary>
        ///     Gets or sets the minimum length of the current text input.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="MinLength"/> is less than 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="MinLength"/> is greater than <see cref="LargestMaxLength"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="MinLength"/> is greater than <see cref="MaxLength"/>.</exception>
        public int? MinLength
        {
            get => _minLength;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"MinLength must not be less than 0");
                if (value > LargestMaxLength)
                    throw new ArgumentOutOfRangeException(nameof(value), $"MinLength must not be greater than {LargestMaxLength}");
                if (value > (MaxLength ?? LargestMaxLength))
                    throw new ArgumentOutOfRangeException(nameof(value), $"MinLength must be less than MaxLength");
                _minLength = value;
            }
        }

        /// <summary>
        ///     Gets or sets the maximum length of the current text input.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="MaxLength"/> is less than 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="MaxLength"/> is greater than <see cref="LargestMaxLength"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="MaxLength"/> is less than <see cref="MinLength"/>.</exception>
        public int? MaxLength
        {
            get => _maxLength;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"MaxLength must not be less than 0");
                if (value > LargestMaxLength)
                    throw new ArgumentOutOfRangeException(nameof(value), $"MaxLength most not be greater than {LargestMaxLength}");
                if (value < (MinLength ?? -1))
                    throw new ArgumentOutOfRangeException(nameof(value), $"MaxLength must be greater than MinLength ({MinLength})");
                _maxLength = value;
            }
        }

        /// <summary>
        ///     Gets or sets whether the user is required to input text.
        /// </summary>
        public bool? Required { get; set; }

        /// <summary>
        ///     Gets or sets the default value of the text input.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="Value"/>.Length is less than 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <see cref="Value"/>.Length is greater than <see cref="LargestMaxLength"/> or <see cref="MaxLength"/>.
        /// </exception>
        public string Value
        {
            get => _value;
            set
            {
                if (value?.Length > (MaxLength ?? LargestMaxLength))
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value must not be longer than {MaxLength ?? LargestMaxLength}.");
                if (value?.Length < (MinLength ?? 0))
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value must not be shorter than {MinLength}");
                _value = value;
            }
        }

        private string _customId;
        private int? _maxLength;
        private int? _minLength;
        private string _placeholder;
        private string _value;

        /// <summary>
        ///     Creates a new instance of a <see cref="TextInputBuilder"/>.
        /// </summary>
        /// <param name="label">The text input's label.</param>
        /// <param name="style">The text input's style.</param>
        /// <param name="customId">The text input's custom id.</param>
        /// <param name="placeholder">The text input's placeholder.</param>
        /// <param name="minLength">The text input's minimum length.</param>
        /// <param name="maxLength">The text input's maximum length.</param>
        /// <param name="required">The text input's required value.</param>
        public TextInputBuilder (string label, string customId, TextInputStyle style = TextInputStyle.Short, string placeholder = null,
            int? minLength = null, int? maxLength = null, bool? required = null, string value = null)
        {
            Label = label;
            Style = style;
            CustomId = customId;
            Placeholder = placeholder;
            MinLength = minLength;
            MaxLength = maxLength;
            Required = required;
            Value = value;
        }

        /// <summary>
        ///     Creates a new instance of a <see cref="TextInputBuilder"/>.
        /// </summary>
        public TextInputBuilder()
        {

        }

        /// <summary>
        ///     Sets the label of the current builder.
        /// </summary>
        /// <param name="label">The value to set.</param>
        /// <returns>The current builder. </returns>
        public TextInputBuilder WithLabel(string label)
        {
            Label = label;
            return this;
        }

        /// <summary>
        ///     Sets the style of the current builder.
        /// </summary>
        /// <param name="style">The value to set.</param>
        /// <returns>The current builder. </returns>
        public TextInputBuilder WithStyle(TextInputStyle style)
        {
            Style = style;
            return this;
        }

        /// <summary>
        ///     Sets the custom id of the current builder.
        /// </summary>
        /// <param name="customId">The value to set.</param>
        /// <returns>The current builder. </returns>
        public TextInputBuilder WithCustomId(string customId)
        {
            CustomId = customId;
            return this;
        }

        /// <summary>
        ///     Sets the placeholder of the current builder.
        /// </summary>
        /// <param name="placeholder">The value to set.</param>
        /// <returns>The current builder. </returns>
        public TextInputBuilder WithPlaceholder(string placeholder)
        {
            Placeholder = placeholder;
            return this;
        }
        
        /// <summary>
        ///     Sets the value of the current builder.
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <returns>The current builder.</returns>
        public TextInputBuilder WithValue(string value)
        {
            Value = value;
            return this;
        }

        /// <summary>
        ///     Sets the minimum length of the current builder.
        /// </summary>
        /// <param name="minLength">The value to set.</param>
        /// <returns>The current builder. </returns>
        public TextInputBuilder WithMinLength(int minLength)
        {
            MinLength = minLength;
            return this;
        }

        /// <summary>
        ///     Sets the maximum length of the current builder.
        /// </summary>
        /// <param name="maxLength">The value to set.</param>
        /// <returns>The current builder. </returns>
        public TextInputBuilder WithMaxLength(int maxLength)
        {
            MaxLength = maxLength;
            return this;
        }

        /// <summary>
        ///     Sets the required value of the current builder.
        /// </summary>
        /// <param name="required">The value to set.</param>
        /// <returns>The current builder. </returns>
        public TextInputBuilder WithRequired(bool required)
        {
            Required = required;
            return this;
        }

        public TextInputComponent Build()
        {
            if (string.IsNullOrEmpty(CustomId))
                throw new ArgumentException("TextInputComponents must have a custom id.", nameof(CustomId));
            if (string.IsNullOrWhiteSpace(Label))
                throw new ArgumentException("TextInputComponents must have a label.", nameof(Label));
            return new TextInputComponent(CustomId, Label, Placeholder, MinLength, MaxLength, Style, Required, Value);
        }
    }
}
