using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
    /// <summary>
    ///     Represents a builder for creating a <see cref="Modal"/>.
    /// </summary>
    public class ModalBuilder
    {
        private string _customId;

        public ModalBuilder() { }

        /// <summary>
        ///     Creates a new instance of the <see cref="ModalBuilder"/>.
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
        ///     Gets or sets the title of the current modal.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the custom ID of the current modal.
        /// </summary>
        public string CustomId
        {
            get => _customId;
            set
            {
                Preconditions.AtLeast(value.Length, 1, nameof(CustomId));
                Preconditions.AtMost(value.Length, ComponentBuilder.MaxCustomIdLength, nameof(CustomId));
                _customId = value;
            }
        }

        /// <summary>
        ///     Gets or sets the components of the current modal.
        /// </summary>
        public ModalComponentBuilder Components { get; set; } = new();

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
        /// <param name="row">The row to add the text input.</param>
        /// <returns>The current builder.</returns>
        public ModalBuilder AddTextInput(TextInputBuilder component, int row = 0)
        {
            Components.WithTextInput(component, row);
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
        ///     Gets a <typeparamref name="TMessageComponent"/> by the specified <paramref name="customId"/>.
        /// </summary>
        /// <typeparam name="TMessageComponent">The type of the component to get.</typeparam>
        /// <param name="customId">The <see cref="IMessageComponent.CustomId"/> of the component to get.</param>
        /// <returns>
        ///     The component of type <typeparamref name="TMessageComponent"/> that was found, <see langword="null"/> otherwise.
        /// </returns>
        public TMessageComponent GetComponent<TMessageComponent>(string customId)
            where TMessageComponent : class, IMessageComponent
        {
            Preconditions.NotNull(customId, nameof(customId));

            return Components.ActionRows
                ?.SelectMany(r => r.Components.OfType<TMessageComponent>())
                .FirstOrDefault(c => c?.CustomId == customId);
        }

        /// <summary>
        ///     Updates a <see cref="TextInputComponent"/> by the specified <paramref name="customId"/>.
        /// </summary>
        /// <param name="customId">The <see cref="TextInputComponent.CustomId"/> of the input to update.</param>
        /// <param name="updateTextInput">An action that configures the updated text input.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when the <see cref="TextInputComponent"/> to be updated was not found.
        /// </exception>
        public ModalBuilder UpdateTextInput(string customId, Action<TextInputBuilder> updateTextInput)
        {
            Preconditions.NotNull(customId, nameof(customId));

            var component = GetComponent<TextInputComponent>(customId) ?? throw new ArgumentException($"There is no component of type {nameof(TextInputComponent)} with the specified custom ID in this modal builder.", nameof(customId));
            var row = Components.ActionRows.First(r => r.Components.Contains(component));

            var builder = new TextInputBuilder
            {
                Label = component.Label,
                CustomId = component.CustomId,
                Style = component.Style,
                Placeholder = component.Placeholder,
                MinLength = component.MinLength,
                MaxLength = component.MaxLength,
                Required = component.Required,
                Value = component.Value
            };

            updateTextInput(builder);

            row.Components.Remove(component);
            row.AddComponent(builder.Build());

            return this;
        }
        
        /// <summary>
        ///     Updates the value of a <see cref="TextInputComponent"/> by the specified <paramref name="customId"/>.
        /// </summary>
        /// <param name="customId">The <see cref="TextInputComponent.CustomId"/> of the input to update.</param>
        /// <param name="value">The new value to put.</param>
        /// <returns>The current builder.</returns>
        public ModalBuilder UpdateTextInput(string customId, object value)
        {
            UpdateTextInput(customId, x => x.Value = value?.ToString());
            return this;
        }

        /// <summary>
        ///     Removes a component from this builder by the specified <paramref name="customId"/>.
        /// </summary>
        /// <param name="customId">The <see cref="IMessageComponent.CustomId"/> of the component to remove.</param>
        /// <returns>The current builder.</returns>
        public ModalBuilder RemoveComponent(string customId)
        {
            Preconditions.NotNull(customId, nameof(customId));

            Components.ActionRows?.ForEach(r => r.Components.RemoveAll(c => c.CustomId == customId));
            return this;
        }

        /// <summary>
        ///     Removes all components of the given <paramref name="type"/> from this builder.
        /// </summary>
        /// <param name="type">The <see cref="ComponentType"/> to remove.</param>
        /// <returns>The current builder.</returns>
        public ModalBuilder RemoveComponentsOfType(ComponentType type)
        {
            Components.ActionRows?.ForEach(r => r.Components.RemoveAll(c => c.Type == type));
            return this;
        }

        /// <summary>
        ///     Builds this builder into a <see cref="Modal"/>.
        /// </summary>
        /// <returns>A <see cref="Modal"/> with the same values as this builder.</returns>
        /// <exception cref="ArgumentException">Modals must have a custom ID.</exception>
        /// <exception cref="ArgumentException">Modals must have a title.</exception>
        /// <exception cref="ArgumentException">Only components of type <see cref="TextInputComponent"/> are allowed.</exception>
        public Modal Build()
        {
            if (string.IsNullOrEmpty(CustomId))
                throw new ArgumentException("Modals must have a custom ID.", nameof(CustomId));
            if (string.IsNullOrWhiteSpace(Title))
                throw new ArgumentException("Modals must have a title.", nameof(Title));
            if (Components.ActionRows?.SelectMany(r => r.Components).Any(c => c.Type != ComponentType.TextInput) ?? false)
                throw new ArgumentException($"Only components of type {nameof(TextInputComponent)} are allowed.", nameof(Components));

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
            => new(ActionRows?.Select(x => x.Build()).ToList());
    }
}
