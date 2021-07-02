using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a builder for creating a <see cref="MessageComponent"/>.
    /// </summary>
    public class ComponentBuilder
    {
        /// <summary>
        ///     The max length of a <see cref="ButtonComponent.Label"/>.
        /// </summary>
        public const int MaxLabelLength = 80;

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
        ///     Adds a <see cref="SelectMenuBuilder"/> to the first row, if the first row cannot
        ///     accept the component then it will add it to a row that can
        /// </summary>
        /// <param name="label">The label of the menu.</param>
        /// <param name="customId">The custom id of the menu.</param>
        /// <param name="options">The options of the menu.</param>
        /// <param name="placeholder">The placeholder of the menu.</param>
        /// <param name="minValues">The min values of the placeholder.</param>
        /// <param name="maxValues">The max values of the placeholder.</param>
        /// <param name="row">The row to add the menu to.</param>
        /// <returns></returns>
        public ComponentBuilder WithSelectMenu(string label, string customId, List<SelectMenuOptionBuilder> options,
            string placeholder = null, int minValues = 1, int maxValues = 1, int row = 0)
        {
            return WithSelectMenu(new SelectMenuBuilder()
                .WithLabel(label)
                .WithCustomId(customId)
                .WithOptions(options)
                .WithPlaceholder(placeholder)
                .WithMaxValues(maxValues)
                .WithMinValues(minValues),
                row);
        }

        /// <summary>
        ///     Adds a <see cref="SelectMenuBuilder"/> to the first row, if the first row cannot
        ///     accept the component then it will add it to a row that can
        /// </summary>
        /// <param name="menu">The menu to add</param>
        /// <returns>The current builder.</returns>
        public ComponentBuilder WithSelectMenu(SelectMenuBuilder menu)
            => WithSelectMenu(menu, 0);

        /// <summary>
        ///     Adds a <see cref="SelectMenuBuilder"/> to the current builder at the specific row.
        /// </summary>
        /// <param name="menu">The menu to add.</param>
        /// <param name="row">The row to attempt to add this component on.</param>
        /// <returns>The current builder.</returns>
        public ComponentBuilder WithSelectMenu(SelectMenuBuilder menu, int row)
        {
            Preconditions.LessThan(row, 5, nameof(row));

            var builtMenu = menu.Build();

            if (_actionRows == null)
            {
                _actionRows = new List<ActionRowBuilder>();
                _actionRows.Add(new ActionRowBuilder().WithComponent(builtMenu));
            }
            else
            {
                if (_actionRows.Count == row)
                    _actionRows.Add(new ActionRowBuilder().WithComponent(builtMenu));
                else
                {
                    ActionRowBuilder actionRow = null;
                    if (_actionRows.Count < row)
                        actionRow = _actionRows.ElementAt(row);
                    else
                    {
                        actionRow = new ActionRowBuilder();
                        _actionRows.Add(actionRow);
                    }

                    if (actionRow.CanTakeComponent(builtMenu))
                        actionRow.WithComponent(builtMenu);
                    else if (row < 5)
                        WithSelectMenu(menu, row + 1);
                    else
                        throw new ArgumentOutOfRangeException($"There is no more room to add a {nameof(builtMenu)}");
                }
            }

            return this;
        }

        /// <summary>
        ///     Adds a button to the specified row.
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
            string label,
            string customId,
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
        ///     Adds a button to the first row.
        /// </summary>
        /// <param name="button">The button to add to the first row.</param>
        /// <returns>The current builder.</returns>
        public ComponentBuilder WithButton(ButtonBuilder button)
            => this.WithButton(button, 0);

        /// <summary>
        ///     Adds a button to the specified row.
        /// </summary>
        /// <param name="button">The button to add.</param>
        /// <param name="row">The row to add the button.</param>
        /// <returns>The current builder.</returns>
        public ComponentBuilder WithButton(ButtonBuilder button, int row)
        {
            Preconditions.LessThan(row, 5, nameof(row));

            var builtButton = button.Build();

            if (_actionRows == null)
            {
                _actionRows = new List<ActionRowBuilder>();
                _actionRows.Add(new ActionRowBuilder().WithComponent(builtButton));
            }
            else
            {
                if (_actionRows.Count == row)
                    _actionRows.Add(new ActionRowBuilder().WithComponent(builtButton));
                else
                {
                    ActionRowBuilder actionRow = null;
                    if(_actionRows.Count < row)
                        actionRow = _actionRows.ElementAt(row);
                    else
                    {
                        actionRow = new ActionRowBuilder();
                        _actionRows.Add(actionRow);
                    }

                    if (actionRow.CanTakeComponent(builtButton))
                        actionRow.WithComponent(builtButton);
                    else if (row < 5)
                        WithButton(button, row + 1);
                    else
                        throw new ArgumentOutOfRangeException($"There is no more room to add a {nameof(button)}");
                }
            }

            return this;
        }

        /// <summary>
        ///     Builds this builder into a <see cref="MessageComponent"/> used to send your components.
        /// </summary>
        /// <returns>A <see cref="MessageComponent"/> that can be sent with <see cref="IMessageChannel.SendMessageAsync(string, bool, Embed, RequestOptions, AllowedMentions, MessageReference, MessageComponent)"/></returns>
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
        public List<IMessageComponent> Components
        {
            get => _components;
            set
            {
                if (value != null)
                    if (value.Count > MaxChildCount)
                        throw new ArgumentException(message: $"Action row can only contain {MaxChildCount} child components!", paramName: nameof(Components));
                _components = value;
            }
        }

        private List<IMessageComponent> _components { get; set; }

        /// <summary>
        ///     Adds a list of components to the current row.
        /// </summary>
        /// <param name="components">The list of components to add.</param>
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
        /// <returns>The current builder.</returns>
        public ActionRowBuilder WithComponent(IMessageComponent component)
        {
            if (this.Components == null)
                this.Components = new List<IMessageComponent>();

            this.Components.Add(component);

            return this;
        }

        /// <summary>
        ///     Builds the current builder to a <see cref="ActionRowComponent"/> that can be used within a <see cref="ComponentBuilder"/>
        /// </summary>
        /// <returns>A <see cref="ActionRowComponent"/> that can be used within a <see cref="ComponentBuilder"/></returns>
        /// <exception cref="ArgumentNullException"><see cref="Components"/> cannot be null.</exception>
        /// <exception cref="ArgumentException">There must be at least 1 component in a row.</exception>
        public ActionRowComponent Build()
        {
            if (this.Components == null)
                throw new ArgumentNullException($"{nameof(Components)} cannot be null!");

            if (this.Components.Count == 0)
                throw new ArgumentException("There must be at least 1 component in a row");

            return new ActionRowComponent(this._components);
        }

        internal bool CanTakeComponent(IMessageComponent component)
        {
            switch (component.Type)
            {
                case ComponentType.ActionRow:
                    return false;
                case ComponentType.Button:
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
        ///     Gets or sets the label of the current button.
        /// </summary>
        public string Label
        {
            get => _label;
            set
            {
                if (value != null)
                    if (value.Length > ComponentBuilder.MaxLabelLength)
                        throw new ArgumentException(message: $"Button label must be {ComponentBuilder.MaxLabelLength} characters or less!", paramName: nameof(Label));

                _label = value;
            }
        }

        /// <summary>
        ///     Gets or sets the custom id of the current button.
        /// </summary>
        public string CustomId
        {
            get => _customId;
            set
            {
                if (value != null)
                    if (value.Length > ComponentBuilder.MaxCustomIdLength)
                        throw new ArgumentException(message: $"Custom Id must be {ComponentBuilder.MaxCustomIdLength} characters or less!", paramName: nameof(CustomId));
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
        ///     Creates a button with the <see cref="ButtonStyle.Link"/> style.
        /// </summary>
        /// <param name="label">The label to use on the newly created link button.</param>
        /// <param name="url">The url for this link button to go to.</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateLinkButton(string label, string url)
        {
            var builder = new ButtonBuilder()
                .WithStyle(ButtonStyle.Link)
                .WithUrl(url)
                .WithLabel(label);

            return builder;
        }

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Danger"/> style.
        /// </summary>
        /// <param name="label">The label for this danger button.</param>
        /// <param name="customId">The custom id for this danger button.</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateDangerButton(string label, string customId)
        {
            var builder = new ButtonBuilder()
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(customId)
                .WithLabel(label);

            return builder;
        }

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Primary"/> style.
        /// </summary>
        /// <param name="label">The label for this primary button.</param>
        /// <param name="customId">The custom id for this primary button.</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreatePrimaryButton(string label, string customId)
        {
            var builder = new ButtonBuilder()
                .WithStyle(ButtonStyle.Primary)
                .WithCustomId(customId)
                .WithLabel(label);

            return builder;
        }

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Secondary"/> style.
        /// </summary>
        /// <param name="label">The label for this secondary button.</param>
        /// <param name="customId">The custom id for this secondary button.</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateSecondaryButton(string label, string customId)
        {
            var builder = new ButtonBuilder()
                .WithStyle(ButtonStyle.Secondary)
                .WithCustomId(customId)
                .WithLabel(label);

            return builder;
        }

        /// <summary>
        ///     Creates a button with the <see cref="ButtonStyle.Success"/> style.
        /// </summary>
        /// <param name="label">The label for this success button.</param>
        /// <param name="customId">The custom id for this success button.</param>
        /// <returns>A builder with the newly created button.</returns>
        public static ButtonBuilder CreateSuccessButton(string label, string customId)
        {
            var builder = new ButtonBuilder()
                .WithStyle(ButtonStyle.Success)
                .WithCustomId(customId)
                .WithLabel(label);

            return builder;
        }

        /// <summary>
        ///     Sets the current buttons label to the specified text.
        /// </summary>
        /// <param name="label">The text for the label</param>
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
        /// <exception cref="InvalidOperationException">A button cannot contain a URL and a CustomId.</exception>
        /// <exception cref="ArgumentException">A button must have an Emote or a label.</exception>
        public ButtonComponent Build()
        {
            if (string.IsNullOrEmpty(this.Label) && this.Emote == null)
                throw new ArgumentException("A button must have an Emote or a label!");

            if (!string.IsNullOrEmpty(this.Url) && !string.IsNullOrEmpty(this.CustomId))
                throw new InvalidOperationException("A button cannot contain a URL and a CustomId");

            if (this.Style == ButtonStyle.Link && !string.IsNullOrEmpty(this.CustomId))
                this.CustomId = null;

            else if (this.Style != ButtonStyle.Link && !string.IsNullOrEmpty(this.Url)) // Thanks 𝑴𝒓𝑪𝒂𝒌𝒆𝑺𝒍𝒂𝒚𝒆𝒓 :D
                this.Url = null;

            return new ButtonComponent(this.Style, this.Label, this.Emote, this.CustomId, this.Url, this.Disabled);
        }
    }

    /// <summary>
    ///     Represents a class used to build <see cref="SelectMenu"/>'s.
    /// </summary>
    public class SelectMenuBuilder
    {
        /// <summary>
        ///     The max length of a <see cref="SelectMenu.Placeholder"/>.
        /// </summary>
        public const int MaxPlaceholderLength = 100;

        /// <summary>
        ///     The maximum number of values for the <see cref="SelectMenu.MinValues"/> and <see cref="SelectMenu.MaxValues"/> properties.
        /// </summary>
        public const int MaxValuesCount = 25;

        /// <summary>
        ///     The maximum number of options a <see cref="SelectMenu"/> can have.
        /// </summary>
        public const int MaxOptionCount = 25;

        /// <summary>
        ///     Gets or sets the label of the current select menu.
        /// </summary>
        public string Label
        {
            get => _label;
            set
            {
                if (value != null)
                    if (value.Length > ComponentBuilder.MaxLabelLength)
                        throw new ArgumentException(message: $"Button label must be {ComponentBuilder.MaxLabelLength} characters or less!", paramName: nameof(Label));

                _label = value;
            }
        }

        /// <summary>
        ///     Gets or sets the custom id of the current select menu.
        /// </summary>
        public string CustomId
        {
            get => _customId;
            set
            {
                if (value != null)
                    if (value.Length > ComponentBuilder.MaxCustomIdLength)
                        throw new ArgumentException(message: $"Custom Id must be {ComponentBuilder.MaxCustomIdLength} characters or less!", paramName: nameof(CustomId));
                _customId = value;
            }
        }

        /// <summary>
        ///     Gets or sets the placeholder text of the current select menu.
        /// </summary>
        public string Placeholder
        {
            get => _placeholder;
            set
            {
                if (value?.Length > MaxPlaceholderLength)
                    throw new ArgumentException(message: $"Placeholder must be {MaxPlaceholderLength} characters or less!", paramName: nameof(Placeholder));

                _placeholder = value;
            }
        }

        /// <summary>
        ///     Gets or sets the minimum values of the current select menu.
        /// </summary>
        public int MinValues
        {
            get => _minvalues;
            set
            {
                Preconditions.LessThan(value, MaxValuesCount, nameof(MinValues));
                _minvalues = value;
            }
        }

        /// <summary>
        ///     Gets or sets the maximum values of the current select menu.
        /// </summary>
        public int MaxValues
        {
            get => _maxvalues;
            set
            {
                Preconditions.LessThan(value, MaxValuesCount, nameof(MaxValues));
                _maxvalues = value;
            }
        }

        /// <summary>
        ///     Gets or sets a collection of <see cref="SelectMenuOptionBuilder"/> for this current select menu.
        /// </summary>
        public List<SelectMenuOptionBuilder> Options
        {
            get => _options;
            set
            {
                if (value != null)
                    Preconditions.LessThan(value.Count, MaxOptionCount, nameof(Options));

                _options = value;
            }
        }

        private List<SelectMenuOptionBuilder> _options;
        private int _minvalues = 1;
        private int _maxvalues = 1;
        private string _placeholder;
        private string _label;
        private string _customId;

        /// <summary>
        ///     Creates a new instance of a <see cref="SelectMenuBuilder"/>.
        /// </summary>
        public SelectMenuBuilder() { }

        /// <summary>
        ///     Creates a new instance of a <see cref="SelectMenuBuilder"/>.
        /// </summary>
        /// <param name="customId">The custom id of this select menu.</param>
        /// <param name="options">The options for this select menu.</param>
        public SelectMenuBuilder(string customId, List<SelectMenuOptionBuilder> options)
        {
            this.CustomId = customId;
            this.Options = options;
        }

        /// <summary>
        ///     Sets the field label.
        /// </summary>
        /// <param name="label">The value to set the field label to.</param>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder WithLabel(string label)
        {
            this.Label = label;
            return this;
        }

        /// <summary>
        ///     Sets the field CustomId.
        /// </summary>
        /// <param name="customId">The value to set the field CustomId to.</param>
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
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SelectMenuBuilder WithOptions(List<SelectMenuOptionBuilder> options)
        {
            this.Options = options;
            return this;
        }

        /// <summary>
        ///     Builds a <see cref="SelectMenu"/>
        /// </summary>
        /// <returns>The newly built <see cref="SelectMenu"/></returns>
        public SelectMenu Build()
        {
            var opt = this.Options?.Select(x => x.Build()).ToList();

            return new SelectMenu(this.CustomId, opt, this.Placeholder, this.MinValues, this.MaxValues);
        }
    }

    /// <summary>
    ///     Represents a class used to build <see cref="SelectMenuOption"/>'s.
    /// </summary>
    public class SelectMenuOptionBuilder
    {
        /// <summary>
        ///     The maximum length of a <see cref="SelectMenuOption.Description"/>.
        /// </summary>
        public const int MaxDescriptionLength = 50;

        /// <summary>
        ///     Gets or sets the label of the current select menu.
        /// </summary>
        public string Label
        {
            get => _label;
            set
            {
                if (value != null)
                    if (value.Length > ComponentBuilder.MaxLabelLength)
                        throw new ArgumentException(message: $"Button label must be {ComponentBuilder.MaxLabelLength} characters or less!", paramName: nameof(Label));

                _label = value;
            }
        }

        /// <summary>
        ///     Gets or sets the custom id of the current select menu.
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                if (value != null)
                    if (value.Length > ComponentBuilder.MaxCustomIdLength)
                        throw new ArgumentException(message: $"Value must be {ComponentBuilder.MaxCustomIdLength} characters or less!", paramName: nameof(Value));
                _value = value;
            }
        }

        /// <summary>
        ///     Gets or sets this menu options description.
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if (value != null)
                    Preconditions.LessThan(value.Length, MaxDescriptionLength, nameof(Description));

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
        public SelectMenuOptionBuilder(string label, string value)
        {
            this.Label = label;
            this.Value = value;
        }

        /// <summary>
        ///     Sets the field label.
        /// </summary>
        /// <param name="label">The value to set the field label to.</param>
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
