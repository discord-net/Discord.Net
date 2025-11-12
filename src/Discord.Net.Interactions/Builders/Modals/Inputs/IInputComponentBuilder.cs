namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represent a builder for creating <see cref="InputComponentInfo"/>.
    /// </summary>
    public interface IInputComponentBuilder : IModalComponentBuilder
    {
        /// <summary>
        ///     Gets the custom id of this input component.
        /// </summary>
        string CustomId { get; }

        /// <summary>
        ///     Gets the label of this input component.
        /// </summary>
        string Label { get; }

        /// <summary>
        ///     Gets the label description of this input component.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Gets whether this input component is required.
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        ///     Get the <see cref="ModalComponentTypeConverter"/> assigned to this input.
        /// </summary>
        ModalComponentTypeConverter TypeConverter { get; }
        /// <summary>
        ///     Sets <see cref="CustomId"/>.
        /// </summary>
        /// <param name="customId">New value of the <see cref="CustomId"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IInputComponentBuilder WithCustomId(string customId);

        /// <summary>
        ///     Sets <see cref="Label"/>.
        /// </summary>
        /// <param name="label">New value of the <see cref="Label"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IInputComponentBuilder WithLabel(string label);

        /// <summary>
        ///     Sets <see cref="Description"/>.
        /// </summary>
        /// <param name="description">New value of the <see cref="Description"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IInputComponentBuilder WithDescription(string description);

        /// <summary>
        ///     Sets <see cref="IsRequired"/>.
        /// </summary>
        /// <param name="isRequired">New value of the <see cref="IsRequired"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IInputComponentBuilder SetIsRequired(bool isRequired);
    }
}
