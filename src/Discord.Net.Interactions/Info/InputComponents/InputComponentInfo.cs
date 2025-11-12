namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the base info class for <see cref="IModal"/> input components.
    /// </summary>
    public abstract class InputComponentInfo : ModalComponentInfo
    {
        /// <summary>
        ///     Gets the custom id of this component.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the label of this component.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     Gets the description of this component.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Gets whether or not this component requires a user input.
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        ///     Gets the <see cref="ModalComponentTypeConverter"/> assigned to this component.
        /// </summary>
        public ModalComponentTypeConverter TypeConverter { get; }

        internal InputComponentInfo(Builders.IInputComponentBuilder builder, ModalInfo modal)
            : base(builder, modal)
        {
            CustomId = builder.CustomId;
            Label = builder.Label;
            Description = builder.Description;
            IsRequired = builder.IsRequired;
            TypeConverter = builder.TypeConverter;
        }
    }
}
