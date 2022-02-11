using System;

namespace Discord.Interactions.Builders
{

    /// <summary>
    ///     Represents a builder for creating <see cref="ModalCommandBuilder"/>.
    /// </summary>
    public class ModalCommandParameterBuilder : ParameterBuilder<ModalCommandParameterInfo, ModalCommandParameterBuilder>
    {
        protected override ModalCommandParameterBuilder Instance => this;

        /// <summary>
        ///     Gets the built <see cref="ModalInfo"/> class for this parameter, if <see cref="IsModalParameter"/> is <see langword="true"/>.
        /// </summary>
        public ModalInfo Modal { get; private set; }

        /// <summary>
        ///     Gets whether or not this parameter is an <see cref="IModal"/>.
        /// </summary>
        public bool IsModalParameter => Modal is not null;

        /// <summary>
        ///     Gets the <see cref="TypeReader"/> assigned to this parameter, if <see cref="IsModalParameter"/> is <see langword="true"/>.
        /// </summary>
        public TypeReader TypeReader { get; private set; }

        internal ModalCommandParameterBuilder(ICommandBuilder command) : base(command) { }

        /// <summary>
        ///     Initializes a new <see cref="ModalCommandParameterBuilder"/>.
        /// </summary>
        /// <param name="command">Parent command of this parameter.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="type">Type of this parameter.</param>
        public ModalCommandParameterBuilder(ICommandBuilder command, string name, Type type) : base(command, name, type) { }

        /// <inheritdoc/>
        public override ModalCommandParameterBuilder SetParameterType(Type type)
        {
            if (typeof(IModal).IsAssignableFrom(type))
                Modal = ModalUtils.GetOrAdd(type, Command.Module.InteractionService);
            else
                TypeReader = Command.Module.InteractionService.GetTypeReader(type);

            return base.SetParameterType(type);
        }

        internal override ModalCommandParameterInfo Build(ICommandInfo command) =>
            new(this, command);
    }
}
