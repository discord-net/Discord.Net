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
                    throw new ArgumentNullException(paramName: nameof(ActionRows), message: "Cannot set an component builder's components collection to null.");
                if (value.Count > MaxActionRowCount)
                    throw new ArgumentException(message: $"Action row count must be less than or equal to {MaxActionRowCount}.", paramName: nameof(ActionRows));
                _actionRows = value;
            }
        }

        private List<ActionRowBuilder> _actionRows { get; set; }

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
            for(int i = 0; i != components.Count; i++)
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
                    this.WithButton(button.Label, button.CustomId, button.Style, button.Emote, button.Url, button.Disabled, row);
                    break;
                case ActionRowComponent actionRow:
                    foreach (var cmp in actionRow.Components)
                        AddComponent(cmp, row);
                    break;
                case SelectMenuComponent menu:
                    this.WithSelectMenu(menu.Placeholder, menu.CustomId, menu.Options.Select(x => new SelectMenuOptionBuilder(x.Label, x.Value, x.Description, x.Emote, x.Default)).ToList(), menu.Placeholder, menu.MinValues, menu.MaxValues, menu.Disabled, row);
                    break;
            }
        }

        /// <summary>
        ///     Adds a <see cref="SelectMenuBuilder"/> to the <see cref="ComponentBuilder"/> at the specific row.
        ///     If the row cannot accept the component then it will add it to a row that can.
        /// </summary>
        /// <param name="label">The label of the menu.</param>
        /// <param name="customId">The custom id of the menu.</param>
        /// <param name="options">The options of the menu.</param>
        /// <param name="placeholder">The placeholder of the menu.</param>
        /// <param name="minValues">The min values of the placeholder.</param>
        /// <param name="maxValues">The max values of the placeholder.</param>
        /// <param name="disabled">Whether or not the menu is disabled.</param>
        /// <param name="row">The row to add the menu to.</param>
        /// <returns></returns>
        public ComponentBuilder WithSelectMenu(string label, string customId, List<SelectMenuOptionBuilder> options,
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
            if (menu.Options.Distinct().Count() != menu.Options.Count())
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

            return this.WithButton(button, row);
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
                _actionRows = new List<ActionRowBuilder>();
                _actionRows.Add(new ActionRowBuilder().AddComponent(builtButton));
            }
            else
            {
                if (_actionRows.Count == row)
                    _actionRows.Add(new ActionRowBuilder().AddComponent(builtButton));
                else
                {
                    ActionRowBuilder actionRow;
                    if(_actionRows.Count > row)
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
        ///     Builds this builder into a <see cref="MessageComponent"/> used to send your components.
        /// </summary>
        /// <returns>A <see cref="MessageComponent"/> that can be sent with <see cref="IMessageChannel.SendMessageAsync(string, bool, Embed, RequestOptions, AllowedMentions, MessageReference, MessageComponent)"/>.</returns>
        public MessageComponent Build()
        {
            if (this._actionRows != null)
                return new MessageComponent(this._actionRows.Select(x => x.Build()).ToList());
            else
                return MessageComponent.Empty;
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
                    throw new ArgumentNullException(message: "Action row components cannot be null!", paramName: nameof(Components));

                if (value.Count <= 0)
                    throw new ArgumentException(message: "There must be at least 1 component in a row", paramName: nameof(Components));

                if (value.Count > MaxChildCount)
                    throw new ArgumentException(message: $"Action row can only contain {MaxChildCount} child components!", paramName: nameof(Components));

                _components = value;
            }
        }

        private List<IMessageComponent> _components { get; set; } = new List<IMessageComponent>();

        /// <summary>
        ///     Adds a list of components to the current row.
        /// </summary>
        /// <param name="components">The list of components to add.</param>
        /// <inheritdoc cref="Components"/>
        /// <returns>The current builder.</returns>
        public ActionRowBuilder WithComponents(List<IMessageComponent> components)
        {
            this.Components = components;
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
            if (this.Components.Count >= MaxChildCount)
                throw new InvalidOperationException($"Components count reached {MaxChildCount}");

            this.Components.Add(component);
            return this;
        }

        /// <summary>
        ///     Builds the current builder to a <see cref="ActionRowComponent"/> that can be used within a <see cref="ComponentBuilder"/>
        /// </summary>
        /// <returns>A <see cref="ActionRowComponent"/> that can be used within a <see cref="ComponentBuilder"/></returns>
        public ActionRowComponent Build()
        {
            return new ActionRowComponent(this._components);
        }

        internal bool CanTakeComponent(IMessageComponent component)
        {
            switch (component.Type)
            {
                case ComponentType.ActionRow:
                    return false;
                case ComponentType.Button:
                    if (this.Components.Any(x => x.Type == ComponentType.SelectMenu))
                        return false;
                    else
                        return this.Components.Count < 5;
                case ComponentType.SelectMenu:
                    return this.Components.Count == 0;
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
            set
            {
                if (value != null)
                {
                    if (value.Length > MaxButtonLabelLength)
                        throw new ArgumentException($"Button label must be {MaxButtonLabelLength} characters or less!", paramName: nameof(Label));
                    if (value.Length < 1)
                        throw new ArgumentException("Button label must be 1 character or more!", paramName: nameof(Label));
                }

                _label = value;
            }
        }

        /// <summary>
        ///     Gets or sets the custom id of the current button.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length exceeds <see cref="ComponentBuilder.MaxCustomIdLength"/></exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length subceeds 1.</exception>
        public string CustomId
        {
            get => _customId;
            set
            {
                if (value != null)
                {
                    if (value.Length > ComponentBuilder.MaxCustomIdLength)
                        throw new ArgumentException($"Custom Id must be {ComponentBuilder.MaxCustomIdLength} characters or less!", paramName: nameof(CustomId));
                    if (value.Length < 1)
                        throw new ArgumentException("Custom Id must be 1 character or more!", paramName: nameof(CustomId));
                }
                _customId = value;
            }
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
        public bool Disabled { get; set; }


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
        /// <param name="customId">The custom ID of this button</param>
        /// <param name="style">The custom ID of this button</param>
        /// <param name="emote">The emote of this button</param>
        /// <param name="disabled">Disabled this button or not</param>
        public ButtonBuilder(string label = null, string customId = null, ButtonStyle style = ButtonStyle.Primary, string url = null, IEmote emote = null, bool disabled = false)
        {
            this.CustomId = customId;
            this.Style = style;
            this.Url = url;
            this.Label = label;
            this.Disabled = disabled;
            this.Emote = emote;
        }

        /// <summary>
        ///     Creates a new instance of a <see cref="ButtonBuilder"/> from instance of a <see cref="ButtonComponent"/>.
        /// </summary>
        public ButtonBuilder(ButtonComponent button)
        {
            this.CustomId = button.CustomId;
            this.Style = button.Style;
            this.Url = button.Url;
            this.Label = button.Label;
            this.Disabled = button.Disabled;
            this.Emote = button.Emote;
        }

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Link"/> style.
        /// </summary>
        /// <param name="label">The label for this link button.</param>
        /// <param name="url">The url for this link button to go to.</param>
        /// <param name="emote">The emote for this link button</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateLinkButton(string label, string url, IEmote emote = null)
            => new ButtonBuilder(label, null, ButtonStyle.Link, url, emote: emote);

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Danger"/> style.
        /// </summary>
        /// <param name="label">The label for this danger button.</param>
        /// <param name="customId">The custom id for this danger button.</param>
        /// <param name="emote">The emote for this danger button</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateDangerButton(string label, string customId, IEmote emote = null)
            => new ButtonBuilder(label, customId, ButtonStyle.Danger, emote: emote);

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Primary"/> style.
        /// </summary>
        /// <param name="label">The label for this primary button.</param>
        /// <param name="customId">The custom id for this primary button.</param>
        /// <param name="emote">The emote for this primary button</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreatePrimaryButton(string label, string customId, IEmote emote = null)
            => new ButtonBuilder(label, customId, emote: emote);

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Secondary"/> style.
        /// </summary>
        /// <param name="label">The label for this secondary button.</param>
        /// <param name="customId">The custom id for this secondary button.</param>
        /// <param name="emote">The emote for this secondary button</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateSecondaryButton(string label, string customId, IEmote emote = null)
            => new ButtonBuilder(label, customId, ButtonStyle.Secondary, emote: emote);

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Success"/> style.
        /// </summary>
        /// <param name="label">The label for this success button.</param>
        /// <param name="customId">The custom id for this success button.</param>
        /// <param name="emote">The emote for this success button</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateSuccessButton(string label, string customId, IEmote emote = null)
            => new ButtonBuilder(label, customId, ButtonStyle.Success, emote: emote);

        /// <summary>
        ///     Sets the current buttons label to the specified text.
        /// </summary>
        /// <param name="label">The text for the label</param>
        /// <inheritdoc cref="Label"/>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithLabel(string label)
        {
            this.Label = label;
            return this;
        }

        /// <summary>
        ///     Sets the current buttons style.
        /// </summary>
        /// <param name="style">The style for this builders button.</param>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithStyle(ButtonStyle style)
        {
            this.Style = style;
            return this;
        }

        /// <summary>
        ///     Sets the current buttons emote.
        /// </summary>
        /// <param name="emote">The emote to use for the current button.</param>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithEmote(IEmote emote)
        {
            this.Emote = emote;
            return this;
        }

        /// <summary>
        ///     Sets the current buttons url.
        /// </summary>
        /// <param name="url">The url to use for the current button.</param>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithUrl(string url)
        {
            this.Url = url;
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
            this.CustomId = id;
            return this;
        }

        /// <summary>
        ///     Sets whether the current button is disabled.
        /// </summary>
        /// <param name="disabled">Whether the current button is disabled or not.</param>
        /// <returns>The current builder.</returns>
        public ButtonBuilder WithDisabled(bool disabled)
        {
            this.Disabled = disabled;
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
            if (string.IsNullOrEmpty(this.Label) && this.Emote == null)
                throw new InvalidOperationException("A button must have an Emote or a label!");

            if (!(string.IsNullOrEmpty(this.Url) ^ string.IsNullOrEmpty(this.CustomId)))
                throw new InvalidOperationException("A button must contain either a URL or a CustomId, but not both!");

            if (this.Style == ButtonStyle.Link)
            {
                if (string.IsNullOrEmpty(this.Url))
                    throw new InvalidOperationException("Link buttons must have a link associated with them");
                else
                    UrlValidation.Validate(this.Url);
            }
            else if (string.IsNullOrEmpty(this.CustomId))
                throw new InvalidOperationException("Non-link buttons must have a custom id associated with them");

            return new ButtonComponent(this.Style, this.Label, this.Emote, this.CustomId, this.Url, this.Disabled);
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
            set
            {
                if (value != null)
                {
                    if (value.Length > ComponentBuilder.MaxCustomIdLength)
                        throw new ArgumentException($"Custom Id must be {ComponentBuilder.MaxCustomIdLength} characters or less!", paramName: nameof(CustomId));
                    if (value.Length < 1)
                        throw new ArgumentException("Custom Id must be 1 character or more!", paramName: nameof(CustomId));
                }
                _customId = value;
            }
        }

        /// <summary>
        ///     Gets or sets the placeholder text of the current select menu.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Placeholder"/> length exceeds <see cref="MaxPlaceholderLength"/>.</exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Placeholder"/> length subceeds 1.</exception>
        public string Placeholder
        {
            get => _placeholder;
            set
            {
                if (value != null)
                {
                    if (value.Length > MaxPlaceholderLength)
                        throw new ArgumentException($"The placeholder must be {MaxPlaceholderLength} characters or less!", paramName: nameof(Placeholder));
                    if (value.Length < 1)
                        throw new ArgumentException("The placeholder must be 1 character or more!", paramName: nameof(Placeholder));
                }

                _placeholder = value;
            }
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
                Preconditions.LessThan(value, MaxValuesCount, nameof(MinValues));
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
                Preconditions.LessThan(value, MaxValuesCount, nameof(MaxValues));
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
                    Preconditions.LessThan(value.Count, MaxOptionCount, nameof(Options));
                else
                    throw new ArgumentNullException(nameof(value));

                _options = value;
            }
        }

        /// <summary>
        ///     Gets or sets whether the current menu is disabled.
        /// </summary>
        public bool Disabled { get; set; }

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
            this.Placeholder = selectMenu.Placeholder;
            this.CustomId = selectMenu.Placeholder;
            this.MaxValues = selectMenu.MaxValues;
            this.MinValues = selectMenu.MinValues;
            this.Disabled = selectMenu.Disabled;
            this.Options = selectMenu.Options?
               .Select(x => new SelectMenuOptionBuilder(x.Label, x.Value, x.Description, x.Emote, x.Default))
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
        /// <param name="disabled">Disabled this select menu or not.</param>
        public SelectMenuBuilder(string customId, List<SelectMenuOptionBuilder> options, string placeholder = null, int maxValues = 1, int minValues = 1, bool disabled = false)
        {
            this.CustomId = customId;
            this.Options = options;
            this.Placeholder = placeholder;
            this.Disabled = disabled;
            this.MaxValues = maxValues;
            this.MinValues = minValues;
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
            this.CustomId = customId;
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
            this.Placeholder = placeholder;
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
            this.MinValues = minValues;
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
            this.MaxValues = maxValues;
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
            this.Options = options;
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
            if (this.Options.Count >= MaxOptionCount)
                throw new InvalidOperationException($"Options count reached {MaxOptionCount}.");

            this.Options.Add(option);
            return this;
        }

        /// <summary>
        ///     Add one option to menu options.
        /// </summary>
        /// <param name="label">The label for this option.</param>
        /// <param name="value">The value of this option.</param>
        /// <param name="description">The description of this option.</param>
        /// <param name="emote">The emote of this option.</param>
        /// <param name="default">Render this option as selected by default or not.</param>
        /// <exception cref="InvalidOperationException">Options count reached <see cref="MaxOptionCount"/>.</exception>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder AddOption(string label, string value, string description = null, IEmote emote = null, bool? @default = null)
        {
            AddOption(new SelectMenuOptionBuilder(label, value, description, emote, @default));
            return this;
        }

        /// <summary>
        ///     Sets whether the current menu is disabled.
        /// </summary>
        /// <param name="disabled">Whether the current menu is disabled or not.</param>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder WithDisabled(bool disabled)
        {
            this.Disabled = disabled;
            return this;
        }

        /// <summary>
        ///     Builds a <see cref="SelectMenuComponent"/>
        /// </summary>
        /// <returns>The newly built <see cref="SelectMenuComponent"/></returns>
        public SelectMenuComponent Build()
        {
            var options = this.Options?.Select(x => x.Build()).ToList();

            return new SelectMenuComponent(this.CustomId, options, this.Placeholder, this.MinValues, this.MaxValues, this.Disabled);
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
            set
            {
                if (value != null)
                {
                    if (value.Length > MaxSelectLabelLength)
                        throw new ArgumentException($"Select option label must be {MaxSelectLabelLength} characters or less!", paramName: nameof(Label));
                    if (value.Length < 1)
                        throw new ArgumentException("Select option label must be 1 character or more!", paramName: nameof(Label));
                }

                _label = value;
            }
        }

        /// <summary>
        ///     Gets or sets the custom id of the current select menu.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Value"/> length exceeds <see cref="MaxSelectValueLength"/>.</exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Value"/> length subceeds 1.</exception>
        public string Value
        {
            get => _value;
            set
            {
                if (value != null)
                {
                    if (value.Length > MaxSelectValueLength)
                        throw new ArgumentException($"Select option value must be {MaxSelectValueLength} characters or less!", paramName: nameof(Label));
                    if (value.Length < 1)
                        throw new ArgumentException("Select option value must be 1 character or more!", paramName: nameof(Label));
                }
                else
                    throw new ArgumentException("Select option value must not be null or empty!", paramName: nameof(Label));

                _value = value;
            }
        }

        /// <summary>
        ///     Gets or sets this menu options description.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Description"/> length exceeds <see cref="MaxDescriptionLength"/>.</exception>
        /// <exception cref="ArgumentException" accessor="set"><see cref="Description"/> length subceeds 1.</exception>
        public string Description
        {
            get => _description;
            set
            {
                if (value != null)
                {
                    if (value.Length > MaxDescriptionLength)
                        throw new ArgumentException($"The description must be {MaxDescriptionLength} characters or less!", paramName: nameof(Label));
                    if (value.Length < 1)
                        throw new ArgumentException("The description must be 1 character or more!", paramName: nameof(Label));
                }

                _description = value;
            }
        }

        /// <summary>
        ///     Gets or sets the emote of this option.
        /// </summary>
        public IEmote Emote { get; set; }

        /// <summary>
        ///     Gets or sets the whether or not this option will render selected by default.
        /// </summary>
        public bool? Default { get; set; }

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
        /// <param name="default">Render this option as selected by default or not.</param>
        public SelectMenuOptionBuilder(string label, string value, string description = null, IEmote emote = null, bool? @default = null)
        {
            this.Label = label;
            this.Value = value;
            this.Description = description;
            this.Emote = emote;
            this.Default = @default;
        }

        /// <summary>
        ///     Creates a new instance of a <see cref="SelectMenuOptionBuilder"/> from instance of a <see cref="SelectMenuOption"/>.
        /// </summary>
        public SelectMenuOptionBuilder(SelectMenuOption option)
        {
            this.Label = option.Label;
            this.Value = option.Value;
            this.Description = option.Description;
            this.Emote = option.Emote;
            this.Default = option.Default;
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
            this.Label = label;
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
            this.Value = value;
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
            this.Description = description;
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
            this.Emote = emote;
            return this;
        }

        /// <summary>
        ///     Sets the field default.
        /// </summary>
        /// <param name="defaultValue">The value to set the field default to.</param>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuOptionBuilder WithDefault(bool defaultValue)
        {
            this.Default = defaultValue;
            return this;
        }

        /// <summary>
        ///     Builds a <see cref="SelectMenuOption"/>.
        /// </summary>
        /// <returns>The newly built <see cref="SelectMenuOption"/>.</returns>
        public SelectMenuOption Build()
        {
            return new SelectMenuOption(this.Label, this.Value, this.Description, this.Emote, this.Default);
        }
    }
}
