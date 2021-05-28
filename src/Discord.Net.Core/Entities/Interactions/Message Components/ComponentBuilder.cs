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
            var builtButton = button.Build();

            if (_actionRows == null)
            {
                _actionRows = new List<ActionRowBuilder>();
                _actionRows.Add(new ActionRowBuilder().WithComponent(builtButton));
            }
            else
            {
                if (_actionRows.Count + 1 == row)
                    _actionRows.Add(new ActionRowBuilder().WithComponent(builtButton));
                else
                {
                    if (_actionRows.Count > row)
                        _actionRows[row].WithComponent(builtButton);
                    else
                    {
                        _actionRows.First().WithComponent(builtButton);
                    }
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
        public List<ButtonComponent> Components
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

        private List<ButtonComponent> _components { get; set; }

        /// <summary>
        ///     Adds a list of components to the current row.
        /// </summary>
        /// <param name="components">The list of components to add.</param>
        /// <returns>The current builder.</returns>
        public ActionRowBuilder WithComponents(List<ButtonComponent> components)
        {
            this.Components = components;
            return this;
        }

        /// <summary>
        ///     Adds a component at the end of the current row.
        /// </summary>
        /// <param name="component">The component to add.</param>
        /// <returns>The current builder.</returns>
        public ActionRowBuilder WithComponent(ButtonComponent component)
        {
            if (this.Components == null)
                this.Components = new List<ButtonComponent>();

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
    }

    /// <summary>
    ///     Represents a class used to build <see cref="ButtonComponent"/>'s.
    /// </summary>
    public class ButtonBuilder
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
        ///     Gets or sets the label of the current button.
        /// </summary>
        public string Label
        {
            get => _label;
            set
            {
                if (value != null)
                    if (value.Length > MaxLabelLength)
                        throw new ArgumentException(message: $"Button label must be {MaxLabelLength} characters or less!", paramName: nameof(Label));

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
                    if (value.Length > MaxCustomIdLength)
                        throw new ArgumentException(message: $"Custom Id must be {MaxCustomIdLength} characters or less!", paramName: nameof(CustomId));
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
            else if (!string.IsNullOrEmpty(this.Url))
                this.Url = null;

            return new ButtonComponent(this.Style, this.Label, this.Emote, this.CustomId, this.Url, this.Disabled);
        }
    }
}
