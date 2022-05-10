using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public class ModalBuilder
    {
        /// <summary>
        ///     Gets or sets the components of the current modal.
        /// </summary>
        public ModalComponentBuilder Components { get; set; } = new();

        /// <summary>
        ///     Gets or sets the title of the current modal.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the custom id of the current modal.
        /// </summary>
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

        private string _customId;

        public ModalBuilder() { }

        /// <summary>
        ///     Creates a new instance of a <see cref="ModalBuilder"/>
        /// </summary>
        /// <param name="title">The modal's title.</param>
        /// <param name="customId">The modal's customId.</param>
        /// <param name="components">The modal's components.</param>
        /// <exception cref="ArgumentException">Only TextInputComponents are allowed.</exception>
        public ModalBuilder(string title, string customId, ModalComponentBuilder components = null)
        {
            Title = title;
            CustomId = customId;
            Components = components ?? new();
        }

        /// <summary>
        ///     Sets the title of the current modal.
        /// </summary>
        /// <param name="title">The value to set the title to.</param>
        /// <returns>The current builder.</returns>
        public ModalBuilder WithTitle(string title)
        {
            Title = title;
            return this;
        }

        /// <summary>
        ///     Sets the custom id of the current modal.
        /// </summary>
        /// <param name="customId">The value to set the custom id to.</param>
        /// <returns>The current builder.</returns>
        public ModalBuilder WithCustomId(string customId)
        {
            CustomId = customId;
            return this;
        }

        /// <summary>
        ///     Adds a component to the current builder.
        /// </summary>
        /// <param name="component">The component to add.</param>
        /// <returns>The current builder.</returns>
        public ModalBuilder AddTextInput(TextInputBuilder component)
        {
            Components.WithTextInput(component);
            return this;
        }

        /// <summary>
        ///     Adds a <see cref="TextInputBuilder"/> to the current builder.
        /// </summary>
        /// <param name="customId">The input's custom id.</param>
        /// <param name="label">The input's label.</param>
        /// <param name="placeholder">The input's placeholder text.</param>
        /// <param name="minLength">The input's minimum length.</param>
        /// <param name="maxLength">The input's maximum length.</param>
        /// <param name="style">The input's style.</param>
        /// <returns>The current builder.</returns>
        public ModalBuilder AddTextInput(string label, string customId, TextInputStyle style = TextInputStyle.Short, 
            string placeholder = "", int? minLength = null, int? maxLength = null, bool? required = null, string value = null)
            => AddTextInput(new(label, customId, style, placeholder, minLength, maxLength, required, value));

        /// <summary>
        ///     Adds multiple components to the current builder.
        /// </summary>
        /// <param name="components">The components to add.</param>
        /// <returns>The current builder</returns>
        public ModalBuilder AddComponents(List<IMessageComponent> components, int row)
        {
            components.ForEach(x => Components.AddComponent(x, row));
            return this;
        }

        /// <summary>
        ///     Builds this builder into a <see cref="Modal"/>.
        /// </summary>
        /// <returns>A <see cref="Modal"/> with the same values as this builder.</returns>
        /// <exception cref="ArgumentException">Only TextInputComponents are allowed.</exception>
        /// <exception cref="ArgumentException">Modals must have a custom id.</exception>
        /// <exception cref="ArgumentException">Modals must have a title.</exception>
        public Modal Build()
        {
            if (string.IsNullOrEmpty(CustomId))
                throw new ArgumentException("Modals must have a custom id.", nameof(CustomId));
            if (string.IsNullOrWhiteSpace(Title))
                throw new ArgumentException("Modals must have a title.", nameof(Title));
            if (Components.ActionRows?.SelectMany(x => x.Components).Any(x => x.Type != ComponentType.TextInput) ?? false)
                throw new ArgumentException($"Only TextInputComponents are allowed.", nameof(Components));

            return new(Title, CustomId, Components.Build());
        }
    }

    /// <summary>
    ///     Represents a builder for creating a <see cref="ModalComponent"/>.
    /// </summary>
    public class ModalComponentBuilder
    {
        /// <summary>
        ///     The max length of a <see cref="IMessageComponent.CustomId"/>.
        /// </summary>
        public const int MaxCustomIdLength = 100;

        /// <summary>
        ///     The max amount of rows a <see cref="ModalComponent"/> can have.
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
                case TextInputComponent text:
                    WithTextInput(text.Label, text.CustomId, text.Style, text.Placeholder, text.MinLength, text.MaxLength, row);
                    break;
                case ActionRowComponent actionRow:
                    foreach (var cmp in actionRow.Components)
                        AddComponent(cmp, row);
                    break;
            }
        }

        /// <summary>
        ///     Adds a <see cref="TextInputBuilder"/> to the <see cref="ComponentBuilder"/> at the specific row.
        ///     If the row cannot accept the component then it will add it to a row that can.
        /// </summary>
        /// <param name="customId">The input's custom id.</param>
        /// <param name="label">The input's label.</param>
        /// <param name="placeholder">The input's placeholder text.</param>
        /// <param name="minLength">The input's minimum length.</param>
        /// <param name="maxLength">The input's maximum length.</param>
        /// <param name="style">The input's style.</param>
        /// <returns>The current builder.</returns>
        public ModalComponentBuilder WithTextInput(string label, string customId, TextInputStyle style = TextInputStyle.Short, 
            string placeholder = null, int? minLength = null, int? maxLength = null, int row = 0, bool? required = null,
            string value = null)
            => WithTextInput(new(label, customId, style, placeholder, minLength, maxLength, required, value), row);

        /// <summary>
        ///     Adds a <see cref="TextInputBuilder"/> to the <see cref="ModalComponentBuilder"/> at the specific row.
        ///     If the row cannot accept the component then it will add it to a row that can.
        /// </summary>
        /// <param name="text">The <see cref="TextInputBuilder"/> to add.</param>
        /// <param name="row">The row to add the text input.</param>
        /// <exception cref="InvalidOperationException">There are no more rows to add a text input to.</exception>
        /// <exception cref="ArgumentException"><paramref name="row"/> must be less than <see cref="MaxActionRowCount"/>.</exception>
        /// <returns>The current builder.</returns>
        public ModalComponentBuilder WithTextInput(TextInputBuilder text, int row = 0)
        {
            Preconditions.LessThan(row, MaxActionRowCount, nameof(row));

            var builtButton = text.Build();

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
                        WithTextInput(text, row + 1);
                    else
                        throw new InvalidOperationException($"There are no more rows to add {nameof(text)} to.");
                }
            }

            return this;
        }

        /// <summary>
        ///     Get a <see cref="ModalComponent"/> representing the builder.
        /// </summary>
        /// <returns>A <see cref="ModalComponent"/> representing the builder.</returns>
        public ModalComponent Build()
            => new (ActionRows?.Select(x => x.Build()).ToList());
    }
}
