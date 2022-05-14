using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="ModalInfo"/>.
    /// </summary>
    public class ModalBuilder
    {
        internal readonly InteractionService _interactionService;
        internal readonly List<IInputComponentBuilder> _components;

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
        public IReadOnlyCollection<IInputComponentBuilder> Components => _components;

        internal ModalBuilder(Type type, InteractionService interactionService)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Must be an implementation of {nameof(IModal)}", nameof(type));

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
        public ModalBuilder AddTextComponent(Action<TextInputComponentBuilder> configure)
        {
            var builder = new TextInputComponentBuilder(this);
            configure(builder);
            _components.Add(builder);
            return this;
        }

        internal ModalInfo Build() => new(this);
    }
}
