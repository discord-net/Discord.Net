using System;
using System.Collections;
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

        /// <summary>
        ///     Creates a new and empty <see cref="ModalBuilder"/>.
        /// </summary>
        public ModalBuilder()
        {
            Components = new();
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ModalBuilder"/>.
        /// </summary>
        /// <param name="title">The modal's title.</param>
        /// <param name="customId">The modal's customId.</param>
        /// <param name="components">The modal's components.</param>
        public ModalBuilder(string title, string customId, ModalComponentBuilder components = null)
        {
            Title = title;
            CustomId = customId;
            Components = components ?? new();
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ModalBuilder"/>.
        /// </summary>
        /// <param name="title">The modal's title.</param>
        /// <param name="customId">The modal's customId.</param>
        /// <param name="components">The modal's components.</param>
        public ModalBuilder(string title, string customId, params IEnumerable<IMessageComponentBuilder> components)
            : this(title, customId, new ModalComponentBuilder(components))
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ModalBuilder"/>.
        /// </summary>
        /// <param name="title">The modal's title.</param>
        /// <param name="customId">The modal's customId.</param>
        /// <param name="components">The modal's components.</param>
        public ModalBuilder(string title, string customId, params IEnumerable<IMessageComponent> components)
            : this(title, customId, new ModalComponentBuilder(components))
        {
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
                if (value is not null)
                {
                    Preconditions.AtLeast(value.Length, 1, nameof(CustomId));
                    Preconditions.AtMost(value.Length, ComponentBuilder.MaxCustomIdLength, nameof(CustomId));
                }

                _customId = value;
            }
        }

        /// <summary>
        ///     Gets or sets the components of the current modal.
        /// </summary>
        public ModalComponentBuilder Components { get; set; }

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
        [Obsolete("Modal components no longer have rows", error: false)]
        public ModalBuilder AddTextInput(TextInputBuilder component, int row)
        {
            Components.WithTextInput(component, row);
            return this;
        }

        /// <inheritdoc
        ///     cref="ModalComponentBuilder.WithTextInput(string, string, TextInputStyle, string, int?, int?, int, bool?, string, int?, string, int?)"
        /// />
        /// <returns>The current <see cref="ModalBuilder"/>.</returns>
        public ModalBuilder AddTextInput(
            string label,
            string customId,
            TextInputStyle style = TextInputStyle.Short,
            string placeholder = null,
            int? minLength = null,
            int? maxLength = null,
            bool? required = null,
            string value = null,
            int? id = null,
            string description = null,
            int? labelId = null
        )
        {
            Components.WithTextInput(
                label, customId, style, placeholder, minLength, maxLength, 0, required, value, id, description,
                labelId
            );

            return this;
        }

        /// <inheritdoc cref="ModalComponentBuilder.WithLabel(LabelBuilder)"/>
        /// <returns>The current <see cref="ModalBuilder"/>.</returns>
        public ModalBuilder AddLabel(LabelBuilder label)
        {
            Components.WithLabel(label);
            return this;
        }

        /// <inheritdoc cref="ModalComponentBuilder.WithLabel(string, IMessageComponentBuilder, string, int?)"/>
        /// <returns>The current <see cref="ModalBuilder"/>.</returns>
        public ModalBuilder AddLabel(
            string label,
            IMessageComponentBuilder component,
            string description = null,
            int? id = null
        )
        {
            Components.WithLabel(label, component, description, id);
            return this;
        }

        /// <inheritdoc cref="ModalComponentBuilder.WithSelectMenu(string, string, List{SelectMenuOptionBuilder}, string, int, int, bool, ComponentType, ChannelType[], int?, string, int?)"/>
        /// <returns>The current <see cref="ModalBuilder"/>.</returns>
        public ModalBuilder AddSelectMenu(
            string label,
            string customId,
            List<SelectMenuOptionBuilder> options = null,
            string placeholder = null,
            int minValues = 1,
            int maxValues = 1,
            bool disabled = false,
            ComponentType type = ComponentType.SelectMenu,
            ChannelType[] channelTypes = null,
            int? id = null,
            string description = null,
            int? labelId = null
        )
        {
            Components.WithSelectMenu(
                label,
                customId,
                options,
                placeholder,
                minValues,
                maxValues,
                disabled,
                type,
                channelTypes,
                id,
                description,
                labelId
            );

            return this;
        }

        /// <inheritdoc cref="ModalComponentBuilder.WithSelectMenu(string, SelectMenuBuilder, string, int?)"/>
        /// <returns>The current <see cref="ModalBuilder"/>.</returns>
        public ModalBuilder AddSelectMenu(
            string label,
            SelectMenuBuilder menu,
            string description = null,
            int? labelId = null
        )
        {
            Components.WithSelectMenu(label, menu, description, labelId);
            return this;
        }

        /// <inheritdoc cref="ModalComponentBuilder.WithFileUpload(string, FileUploadComponentBuilder, string, int?)"/>
        /// <returns>The current <see cref="ModalBuilder"/>.</returns>
        public ModalBuilder AddFileUpload(
            string label,
            FileUploadComponentBuilder fileUpload,
            string description = null,
            int? labelId = null
        )
        {
            Components.WithFileUpload(label, fileUpload, description, labelId);
            return this;
        }

        /// <inheritdoc cref="ModalComponentBuilder.WithFileUpload(string, string, int?, int?, bool, int?, string, int?)"/>
        /// <returns>The current <see cref="ModalBuilder"/>.</returns>
        public ModalBuilder AddFileUpload(
            string label,
            string customId,
            int? minValues = null,
            int? maxValues = null,
            bool isRequired = true,
            int? id = null,
            string description = null,
            int? labelId = null
        )
        {
            Components.WithFileUpload(label, customId,  minValues, maxValues, isRequired, id, description, labelId);
            return this;
        }

        /// <summary>
        ///     Adds multiple components to the current builder.
        /// </summary>
        /// <param name="components">The components to add.</param>
        /// <returns>The current builder</returns>
        [Obsolete("Modal components no longer have rows", error: false)]
        public ModalBuilder AddComponents(List<IMessageComponent> components, int row)
        {
            components.ForEach(x => Components.AddComponent(x, row));
            return this;
        }

        /// <summary>
        ///     Adds multiple components to the current builder.
        /// </summary>
        /// <param name="components">The components to add.</param>
        /// <returns>The current builder</returns>
        public ModalBuilder AddComponents(params IEnumerable<IMessageComponentBuilder> components)
        {
            Components.With(components);
            return this;
        }

        /// <summary>
        ///     Gets a <see cref="IInteractableComponentBuilder"/> by the specified <paramref name="customId"/>.
        /// </summary>
        /// <param name="customId">
        ///     The <see cref="IInteractableComponentBuilder.CustomId"/> of the component to get.
        /// </param>
        /// <returns>
        ///     The component that was found, <see langword="null"/> otherwise.
        /// </returns>
        public IInteractableComponentBuilder GetComponent(string customId) =>
            GetComponent<IInteractableComponentBuilder>(customId);

        /// <summary>
        ///     Gets a <typeparamref name="TMessageComponentBuilder"/> by the specified <paramref name="customId"/>.
        /// </summary>
        /// <typeparam name="TMessageComponentBuilder">The type of the component to get.</typeparam>
        /// <param name="customId">
        ///     The <see cref="IInteractableComponentBuilder.CustomId"/> of the component to get.
        /// </param>
        /// <returns>
        ///     The component of type <typeparamref name="TMessageComponentBuilder"/> that was found,
        ///     <see langword="null"/> otherwise.
        /// </returns>
        public TMessageComponentBuilder GetComponent<TMessageComponentBuilder>(string customId)
            where TMessageComponentBuilder : class, IInteractableComponentBuilder
        {
            Preconditions.NotNull(customId, nameof(customId));

            var components = Components.SelectMany(ExtractComponent);

            // optimization: no need for the of type call if we're checking the root type.
            if (typeof(TMessageComponentBuilder) != typeof(IInteractableComponentBuilder))
                components = components.OfType<TMessageComponentBuilder>();

            return (TMessageComponentBuilder)components.FirstOrDefault(x => x.CustomId == customId);

            /*
             * Used to extract depth=1 components from the modal. Allows for the same behaviour of the previous
             * iteration of the builder, whilst adding support for label components.
             *
             * This is not a long-term solution, and can break if more component types are added or nesting is changed.
             */
            static IEnumerable<IInteractableComponentBuilder> ExtractComponent(IMessageComponentBuilder builder)
                => builder switch
                {
                    LabelBuilder { Component: IInteractableComponentBuilder target } => [target],
                    ActionRowBuilder { Components: { } components }
                        => components.OfType<IInteractableComponentBuilder>(),
                    _ => []
                };
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

            var component = GetComponent<TextInputBuilder>(customId) ?? throw new ArgumentException(
                $"There is no component of type {nameof(TextInputComponent)} with the specified custom ID in this modal builder.",
                nameof(customId));

            /*
             * We can just update the instance in-place, we don't need to update the parent here.
             *
             * NOTE:
             *  this does change the behaviour of this function, since in the previous iteration, we would've removed
             *  and re-added the component to/from the row, which has the inverse effect of sliding it to the end of the
             *  row. With this change, we no longer update the position within the row, but I think the position
             *  shifting was an unintended side effect- and therefor a bug.
             */

            updateTextInput(component);

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
        /// <param name="customId">The <see cref="IInteractableComponent.CustomId"/> of the component to remove.</param>
        /// <returns>The current builder.</returns>
        public ModalBuilder RemoveComponent(string customId)
        {
            Preconditions.NotNull(customId, nameof(customId));

            /*
             * This function actually removed any component with the provided custom id, and could remove
             * more than one. To keep this behaviour, the below code attempts to do the same.
             *
             * For reference, this was the old implementation
             * Components.ActionRows?.ForEach(r => r
             *   .Components
             *   .RemoveAll(c => c is IInteractableComponentBuilder ic && ic.CustomId == customId)
             * );
             */

            foreach (var parent in Components.ToArray())
            {
                switch (parent)
                {
                    case LabelBuilder { Component: IInteractableComponentBuilder target } label
                        when target.CustomId == customId:
                        // you cannot have a label without a component, so we actually remove the label here
                        Components.Remove(label);
                        break;
                    case ActionRowBuilder row:
                        row.Components.RemoveAll(x =>
                            x is IInteractableComponentBuilder ic &&
                            ic.CustomId == customId
                        );
                        break;
                }
            }

            return this;
        }

        /// <summary>
        ///     Removes all components of the given <paramref name="type"/> from this builder.
        /// </summary>
        /// <param name="type">The <see cref="ComponentType"/> to remove.</param>
        /// <returns>The current builder.</returns>
        public ModalBuilder RemoveComponentsOfType(ComponentType type)
        {
            foreach (var component in Components.ToArray())
            {
                if (component.Type == type) Components.Remove(component);
            }

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

            return new(Title, CustomId, Components.Build());
        }
    }

    /// <summary>
    ///     Represents a builder for creating a <see cref="ModalComponent"/>.
    /// </summary>
    public class ModalComponentBuilder : IList<IMessageComponentBuilder>
    {
        /// <summary>
        ///     The max length of a <see cref="IInteractableComponent.CustomId"/>.
        /// </summary>
        public const int MaxCustomIdLength = 100;

        /// <summary>
        ///     The max amount of rows a <see cref="ModalComponent"/> can have.
        /// </summary>
        [Obsolete("Modal components no longer support action rows", error: true)]
        public const int MaxActionRowCount = 5;

        /// <summary>
        ///     Gets the number of components in the builder.
        /// </summary>
        public int Count => _components.Count;

        /// <summary>
        ///     Gets or sets the component at the specified index.
        /// </summary>
        /// <param name="index">The index of the component to get or set</param>
        public IMessageComponentBuilder this[int index]
        {
            get => _components[index];
            set
            {
                ValidateComponentBuilder(value);
                _components[index] = value;
            }
        }

        private readonly List<IMessageComponentBuilder> _components;

        /// <summary>
        ///     Constructs an empty <see cref="ModalComponentBuilder"/>.
        /// </summary>
        public ModalComponentBuilder()
        {
            _components = [];
        }

        /// <summary>
        ///     Constructs a <see cref="ModalComponentBuilder"/> with the provided
        ///     <see cref="IMessageComponentBuilder"/>s.
        /// </summary>
        /// <param name="components">The components to add to this <see cref="ModalComponentBuilder"/></param>
        public ModalComponentBuilder(params IEnumerable<IMessageComponentBuilder> components) : this()
        {
            foreach (var component in components)
            {
                Add(component);
            }
        }

        /// <summary>
        ///     Constructs a <see cref="ModalComponentBuilder"/> with the provided
        ///     <see cref="IMessageComponent"/>s.
        /// </summary>
        /// <param name="components">The components to add to this <see cref="ModalComponentBuilder"/></param>
        public ModalComponentBuilder(params IEnumerable<IMessageComponent> components) : this()
        {
            foreach (var component in components)
            {
                Add(component);
            }
        }

        private static void ValidateComponentBuilder(IMessageComponentBuilder builder)
        {
            if (builder is not LabelBuilder and not ActionRowBuilder)
                throw new InvalidOperationException(
                    $"Modal components only allow labels or rows, not {builder.GetType().Name}"
                );
        }

        /// <summary>
        ///     Creates a new builder from the provided list of components.
        /// </summary>
        /// <param name="components">The components to create the builder from.</param>
        /// <returns>The newly created builder.</returns>
        public static ModalComponentBuilder FromComponents(params IEnumerable<IMessageComponent> components)
        {
            var builder = new ModalComponentBuilder();

            foreach (var component in components)
                builder.Add(component);

            return builder;
        }

        [Obsolete("Modal components no longer have rows", error: true)]
        internal ModalComponentBuilder AddComponent(IMessageComponent component, int row)
            => Add(component);

        /// <summary>
        ///     Adds a component to this <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="component">The component to add.</param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder Add(IMessageComponent component)
            => Add(component.ToBuilder());

        /// <summary>
        ///     Adds a component to this <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="component">The component to add.</param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder Add(IMessageComponentBuilder component)
        {
            ValidateComponentBuilder(component);

            _components.Add(component);
            return this;
        }

        /// <summary>
        ///     Sets the components in this builder to the provided <paramref name="components"/>
        /// </summary>
        /// <param name="components">The components to set this builder to.</param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder With(params IEnumerable<IMessageComponentBuilder> components)
        {
            _components.Clear();

            foreach (var component in components)
                Add(component);

            return this;
        }

        /// <summary>
        ///     Adds a <see cref="LabelBuilder"/> to the current <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="label">The <see cref="LabelBuilder"/> to add.</param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder WithLabel(LabelBuilder label)
            => Add(label);

        /// <summary>
        ///     Constructs and adds a <see cref="LabelBuilder"/> to the current <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="label">The label of the <see cref="LabelBuilder"/>.</param>
        /// <param name="component">The component of the <see cref="LabelBuilder"/>.</param>
        /// <param name="description">The description of the <see cref="LabelBuilder"/>.</param>
        /// <param name="id">The id of the <see cref="LabelBuilder"/>.</param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder WithLabel(
            string label,
            IMessageComponentBuilder component,
            string description = null,
            int? id = null
        ) => WithLabel(new(
            label,
            component,
            description,
            id
        ));

        /// <summary>
        ///     Constructs and adds a <see cref="LabelBuilder"/> containing a <see cref="SelectMenuBuilder"/> to the
        ///     current <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="label">The label around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="customId">The custom id of the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="options">The options of the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="placeholder">The placeholder of the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="minValues">The min values of the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="maxValues">The max values of the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="disabled">Whether the <see cref="SelectMenuBuilder"/> is disabled.</param>
        /// <param name="type">The type of the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="channelTypes">The channel types of the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="id">The id of the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="description">The description around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="labelId">
        ///     The id of the <see cref="LabelBuilder"/> wrapping the <see cref="SelectMenuBuilder"/>.
        /// </param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder WithSelectMenu(
            string label,
            string customId,
            List<SelectMenuOptionBuilder> options = null,
            string placeholder = null,
            int minValues = 1,
            int maxValues = 1,
            bool disabled = false,
            ComponentType type = ComponentType.SelectMenu,
            ChannelType[] channelTypes = null,
            int? id = null,
            string description = null,
            int? labelId = null
        ) => WithSelectMenu(
            label,
            new SelectMenuBuilder()
                .WithId(id)
                .WithCustomId(customId)
                .WithOptions(options)
                .WithPlaceholder(placeholder)
                .WithMaxValues(maxValues)
                .WithMinValues(minValues)
                .WithDisabled(disabled)
                .WithType(type)
                .WithChannelTypes(channelTypes),
            description,
            labelId
        );

        /// <summary>
        ///     Constructs and adds a <see cref="LabelBuilder"/> with the provided <see cref="SelectMenuBuilder"/> to
        ///     the current <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="label">The label around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="menu">The menu to add.</param>
        /// <param name="description">The description around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="labelId">
        ///     The id of the <see cref="LabelBuilder"/> wrapping the <see cref="SelectMenuBuilder"/>.
        /// </param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder WithSelectMenu(
            string label,
            SelectMenuBuilder menu,
            string description = null,
            int? labelId = null
        )
        {
            if (menu.Options is not null && menu.Options.Distinct().Count() != menu.Options.Count)
                throw new InvalidOperationException("Please make sure that there is no duplicates values.");

            return WithLabel(
                label,
                menu,
                description,
                labelId
            );
        }

        /// <summary>
        ///     Constructs and adds a <see cref="LabelBuilder"/> with the provided
        ///     <see cref="FileUploadComponentBuilder"/> to the current <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="label">The label around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="fileUpload">The file upload to add.</param>
        /// <param name="description">The description around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="labelId">
        ///     The id of the <see cref="LabelBuilder"/> wrapping the <see cref="SelectMenuBuilder"/>.
        /// </param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder WithFileUpload(
            string label,
            FileUploadComponentBuilder fileUpload,
            string description = null,
            int? labelId = null
        ) => WithLabel(label, fileUpload, description, labelId);

        /// <summary>
        ///     Constructs and adds a <see cref="LabelBuilder"/> with a <see cref="FileUploadComponentBuilder"/>
        ///     to the current <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="label">The label around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="customId">The custom id of the <see cref="FileUploadComponentBuilder"/>.</param>
        /// <param name="minValues">The min values of the <see cref="FileUploadComponentBuilder"/>.</param>
        /// <param name="maxValues">The max values of the <see cref="FileUploadComponentBuilder"/>.</param>
        /// <param name="isRequired">Whether the <see cref="FileUploadComponentBuilder"/> is required.</param>
        /// <param name="id">The id of the <see cref="FileUploadComponentBuilder"/>.</param>
        /// <param name="description">The description around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="labelId">
        ///     The id of the <see cref="LabelBuilder"/> wrapping the <see cref="SelectMenuBuilder"/>.
        /// </param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder WithFileUpload(
            string label,
            string customId,
            int? minValues = null,
            int? maxValues = null,
            bool isRequired = true,
            int? id = null,
            string description = null,
            int? labelId = null
        ) => WithLabel(
            label,
            new FileUploadComponentBuilder(
                customId,
                minValues,
                maxValues,
                isRequired,
                id
            ),
            description,
            labelId
        );

        /// <summary>
        ///     Constructs and adds a <see cref="LabelBuilder"/> with the provided <see cref="TextInputBuilder"/> to
        ///     the current <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="label">The label around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="textInput">The text input to add.</param>
        /// <param name="description">The description around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="labelId">
        ///     The id of the <see cref="LabelBuilder"/> wrapping the <see cref="SelectMenuBuilder"/>.
        /// </param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder WithTextInput(
            string label,
            TextInputBuilder textInput,
            string description = null,
            int? labelId = null
        ) => WithLabel(label, textInput, description, labelId);

        /// <summary>
        ///     Constructs and adds a <see cref="LabelBuilder"/> with the provided <see cref="TextInputBuilder"/> to
        ///     the current <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="text">The text input to add.</param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        [Obsolete("text components must be wrapped in a label", error: false)]
        public ModalComponentBuilder WithTextInput(TextInputBuilder text)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (text.Label is null)
            {
                // TODO: better explain
                throw new ArgumentNullException(
                    nameof(text),
                    "Label cannot be null"
                );
            }

            return WithLabel(
                text.Label,
                text
            );

#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        ///     Constructs and adds a <see cref="LabelBuilder"/> with the provided <see cref="TextInputBuilder"/> to
        ///     the current <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="text">The text input to add.</param>
        /// <param name="row">The row to add the text input to.</param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        [Obsolete("Modal components no longer have rows", error: false)]
        public ModalComponentBuilder WithTextInput(TextInputBuilder text, int row)
            => WithTextInput(text);

        /// <summary>
        ///     Constructs and adds a <see cref="LabelBuilder"/> with a <see cref="TextInputBuilder"/>
        ///     to the current <see cref="ModalComponentBuilder"/>.
        /// </summary>
        /// <param name="label">The label around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="customId">The custom id of the <see cref="TextInputBuilder"/>.</param>
        /// <param name="style">The style of the <see cref="TextInputBuilder"/>.</param>
        /// <param name="placeholder">The placeholder of the <see cref="TextInputBuilder"/>.</param>
        /// <param name="minLength">The min length of the <see cref="TextInputBuilder"/>.</param>
        /// <param name="maxLength">The max length of the <see cref="TextInputBuilder"/>.</param>
        /// <param name="row"><b>DEPRECATED:</b> The row to place the <see cref="TextInputBuilder"/> on.</param>
        /// <param name="required">Whether the <see cref="TextInputBuilder"/> is required.</param>
        /// <param name="value">The value of the <see cref="TextInputBuilder"/>.</param>
        /// <param name="id">The id of the <see cref="TextInputBuilder"/>.</param>
        /// <param name="description">The description around the <see cref="SelectMenuBuilder"/>.</param>
        /// <param name="labelId">
        ///     The id of the <see cref="LabelBuilder"/> wrapping the <see cref="SelectMenuBuilder"/>.
        /// </param>
        /// <returns>The current <see cref="ModalComponentBuilder"/>.</returns>
        public ModalComponentBuilder WithTextInput(
            string label,
            string customId,
            TextInputStyle style = TextInputStyle.Short,
            string placeholder = null,
            int? minLength = null,
            int? maxLength = null,
            int row = 0,
            bool? required = null,
            string value = null,
            int? id = null,
            string description = null,
            int? labelId = null
        ) => WithLabel(
            label,
            new TextInputBuilder(
                customId,
                style,
                placeholder,
                minLength,
                maxLength,
                required,
                value,
                id
            ),
            description,
            labelId
        );

        /// <inheritdoc />
        void ICollection<IMessageComponentBuilder>.Add(IMessageComponentBuilder item) => Add(item);

        /// <inheritdoc />
        public void Clear() => _components.Clear();

        /// <inheritdoc />
        public bool Contains(IMessageComponentBuilder item) => _components.Contains(item);

        /// <inheritdoc />
        public void CopyTo(IMessageComponentBuilder[] array, int arrayIndex) => _components.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove(IMessageComponentBuilder item) => _components.Remove(item);

        /// <inheritdoc />
        public int IndexOf(IMessageComponentBuilder item) => _components.IndexOf(item);

        /// <inheritdoc />
        public void Insert(int index, IMessageComponentBuilder item)
        {
            ValidateComponentBuilder(item);

            _components.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index) => _components.RemoveAt(index);

        /// <inheritdoc />
        public IEnumerator<IMessageComponentBuilder> GetEnumerator() => _components.GetEnumerator();

        /// <summary>
        ///     Get a <see cref="ModalComponent"/> representing the builder.
        /// </summary>
        /// <returns>A <see cref="ModalComponent"/> representing the builder.</returns>
        public ModalComponent Build()
            => new(_components.Select(x => x.Build()).ToList());

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_components).GetEnumerator();
        bool ICollection<IMessageComponentBuilder>.IsReadOnly => false;
    }
}
