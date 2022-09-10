using Discord.Interactions.Builders;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the base parameter info class for <see cref="InteractionService"/> modals.
    /// </summary>
    public class ModalCommandParameterInfo : CommandParameterInfo
    {
        /// <summary>
        ///     Gets the <see cref="ModalInfo"/> class for this parameter if <see cref="IsModalParameter"/> is true.
        /// </summary>
        public ModalInfo Modal { get; private set; }

        /// <summary>
        ///     Gets whether this parameter is an <see cref="IModal"/>
        /// </summary>
        public bool IsModalParameter { get; }

        /// <summary>
        ///     Gets the <see cref="TypeReader"/> assigned to this parameter, if <see cref="IsModalParameter"/> is <see langword="true"/>.
        /// </summary>
        public TypeReader TypeReader { get; }

        /// <inheritdoc/>
        public new ModalCommandInfo Command => base.Command as ModalCommandInfo;

        internal ModalCommandParameterInfo(ModalCommandParameterBuilder builder, ICommandInfo command) : base(builder, command)
        {
            Modal = builder.Modal;
            IsModalParameter = builder.IsModalParameter;
            TypeReader = builder.TypeReader;
        }
    }
}
