using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents the base builder class for creating <see cref="InputComponentInfo"/>.
    /// </summary>
    /// <typeparam name="TInfo">The <see cref="InputComponentInfo"/> this builder yields when built.</typeparam>
    /// <typeparam name="TBuilder">Inherited <see cref="InputComponentBuilder{TInfo, TBuilder}"/> type.</typeparam>
    public abstract class InputComponentBuilder<TInfo, TBuilder> : ModalComponentBuilder<TInfo, TBuilder>, IInputComponentBuilder
        where TInfo : InputComponentInfo
        where TBuilder : InputComponentBuilder<TInfo, TBuilder>
    {
        private readonly List<Attribute> _attributes;

        /// <inheritdoc/>
        public string CustomId { get; set; }

        /// <inheritdoc/>
        public string Label { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public bool IsRequired { get; set; } = true;

        /// <inheritdoc/>
        public ModalComponentTypeConverter TypeConverter { get; private set; }

        /// <summary>
        ///     Creates an instance of <see cref="InputComponentBuilder{TInfo, TBuilder}"/>
        /// </summary>
        /// <param name="modal">Parent modal of this input component.</param>
        internal InputComponentBuilder(ModalBuilder modal) : base(modal)
        {
            _attributes = new();
        }

        /// <summary>
        ///     Sets <see cref="CustomId"/>.
        /// </summary>
        /// <param name="customId">New value of the <see cref="CustomId"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder WithCustomId(string customId)
        {
            CustomId = customId;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="Label"/>.
        /// </summary>
        /// <param name="label">New value of the <see cref="Label"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder WithLabel(string label)
        {
            Label = label;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="Description"/>.
        /// </summary>
        /// <param name="description">New value of the <see cref="Description"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder WithDescription(string description)
        {
            Description = description;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="IsRequired"/>.
        /// </summary>
        /// <param name="isRequired">New value of the <see cref="IsRequired"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder SetIsRequired(bool isRequired)
        {
            IsRequired = isRequired;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="Type"/>.
        /// </summary>
        /// <param name="type">New value of the <see cref="Type"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public override TBuilder WithType(Type type)
        {
            TypeConverter = Modal._interactionService.GetModalInputTypeConverter(type);
            return base.WithType(type);
        }

        /// <inheritdoc/>
        IInputComponentBuilder IInputComponentBuilder.WithCustomId(string customId) => WithCustomId(customId);

        /// <inheritdoc/>
        IInputComponentBuilder IInputComponentBuilder.WithLabel(string label) => WithLabel(label);

        /// <inheritdoc/>
        IInputComponentBuilder IInputComponentBuilder.WithDescription(string description) => WithDescription(description);

        /// <inheritdoc/>
        IInputComponentBuilder IInputComponentBuilder.SetIsRequired(bool isRequired) => SetIsRequired(isRequired);
    }
}
