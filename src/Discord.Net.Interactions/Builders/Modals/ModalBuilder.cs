using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="ModalInfo"/>.
    /// </summary>
    public class ModalBuilder
    {
        internal readonly InteractionService _interactionService;
        internal readonly List<IModalComponentBuilder> _components;

        /// <summary>
        ///     Gets the initialization delegate for this modal.
        /// </summary>
        public ModalInitializer ModalInitializer { get; internal set; }

        /// <summary>
        ///     Gets the title of this modal.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets the <see cref="IModal"/> implementation used to initialize this object.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Gets a collection of the components of this modal.
        /// </summary>
        public IReadOnlyCollection<IModalComponentBuilder> Components => _components.AsReadOnly();

        internal ModalBuilder(Type type, InteractionService interactionService)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Must be an implementation of {nameof(IModal)}", nameof(type));

            Type = type;

            _interactionService = interactionService;
            _components = new();
        }

        /// <summary>
        ///     Initializes a new <see cref="ModalBuilder"/>
        /// </summary>
        /// <param name="modalInitializer">The initialization delegate for this modal.</param>
        public ModalBuilder(Type type, ModalInitializer modalInitializer, InteractionService interactionService) : this(type, interactionService)
        {
            ModalInitializer = modalInitializer;
        }

        /// <summary>
        ///     Sets <see cref="Title"/>.
        /// </summary>
        /// <param name="title">New value of the <see cref="Title"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder WithTitle(string title)
        {
            Title = title;
            return this;
        }

        /// <summary>
        ///     Adds text components to <see cref="Components"/>.
        /// </summary>
        /// <param name="configure">Text Component builder factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder AddTextInputComponent(Action<TextInputComponentBuilder> configure)
        {
            var builder = new TextInputComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        /// <summary>
        ///     Adds a select menu component to <see cref="Components"/>.
        /// </summary>
        /// <param name="configure">Select menu component builder factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder AddSelectMenuInputComponent(Action<SelectMenuComponentBuilder> configure)
        {
            var builder = new SelectMenuComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        /// <summary>
        ///     Adds a user select component to <see cref="Components"/>.
        /// </summary>
        /// <param name="configure">User select component builder factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder AddUserSelectInputComponent(Action<UserSelectComponentBuilder> configure)
        {
            var builder = new UserSelectComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        /// <summary>
        ///     Adds a role select component to <see cref="Components"/>.
        /// </summary>
        /// <param name="configure">Role select component builder factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder AddRoleSelectInputComponent(Action<RoleSelectComponentBuilder> configure)
        {
            var builder = new RoleSelectComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        /// <summary>
        ///     Adds a mentionable select component to <see cref="Components"/>.
        /// </summary>
        /// <param name="configure">Mentionable select component builder factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder AddMentionableSelectInputComponent(Action<MentionableSelectComponentBuilder> configure)
        {
            var builder = new MentionableSelectComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        /// <summary>
        ///     Adds a channel select component to <see cref="Components"/>.
        /// </summary>
        /// <param name="configure">Channel select component builder factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder AddChannelSelectInputComponent(Action<ChannelSelectComponentBuilder> configure)
        {
            var builder = new ChannelSelectComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        /// <summary>
        ///     Adds a file upload component to <see cref="Components"/>.
        /// </summary>
        /// <param name="configure">File upload component builder factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder AddFileUploadInputComponent(Action<FileUploadComponentBuilder> configure)
        {
            var builder = new FileUploadComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        /// <summary>
        ///     Adds a text display component to <see cref="Components"/>.
        /// </summary>
        /// <param name="configure">Text display component builder factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder AddTextDisplayComponent(Action<TextDisplayComponentBuilder> configure)
        {
            var builder = new TextDisplayComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        public ModalBuilder AddCheckboxComponent(Action<CheckboxComponentBuilder> configure)
        {
            var builder = new CheckboxComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        public ModalBuilder AddCheckboxGroupComponent(Action<CheckboxGroupComponentBuilder> configure)
        {
            var builder = new CheckboxGroupComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        public ModalBuilder AddRadioGroupComponent(Action<RadioGroupComponentBuilder> configure)
        {
            var builder = new RadioGroupComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        internal ModalInfo Build() => new(this);
    }
}
