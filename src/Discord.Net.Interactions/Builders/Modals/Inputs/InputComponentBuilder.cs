using System;
using System.Collections.Generic;
using System.Reflection;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents the base builder class for creating <see cref="InputComponentInfo"/>.
    /// </summary>
    /// <typeparam name="TInfo">The <see cref="InputComponentInfo"/> this builder yields when built.</typeparam>
    /// <typeparam name="TBuilder">Inherited <see cref="InputComponentBuilder{TInfo, TBuilder}"/> type.</typeparam>
    public abstract class InputComponentBuilder<TInfo, TBuilder> : IInputComponentBuilder
        where TInfo : InputComponentInfo
        where TBuilder : InputComponentBuilder<TInfo, TBuilder>
    {
        private readonly List<Attribute> _attributes;
        protected abstract TBuilder Instance { get; }

        /// <inheritdoc/>
        public ModalBuilder Modal { get; }

        /// <inheritdoc/>
        public string CustomId { get; set; }

        /// <inheritdoc/>
        public string Label { get; set; }

        /// <inheritdoc/>
        public bool IsRequired { get; set; } = true;

        /// <inheritdoc/>
        public ComponentType ComponentType { get; internal set; }

        /// <inheritdoc/>
        public Type Type { get; private set; }

        /// <inheritdoc/>
        public PropertyInfo PropertyInfo { get; internal set; }

        /// <inheritdoc/>
        public ComponentTypeConverter TypeConverter { get; private set; }

        /// <inheritdoc/>
        public object DefaultValue { get; set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes => _attributes;

        /// <summary>
        ///     Creates an instance of <see cref="InputComponentBuilder{TInfo, TBuilder}"/>
        /// </summary>
        /// <param name="modal">Parent modal of this input component.</param>
        public InputComponentBuilder(ModalBuilder modal)
        {
            Modal = modal;
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
        ///     Sets <see cref="ComponentType"/>.
        /// </summary>
        /// <param name="componentType">New value of the <see cref="ComponentType"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder WithComponentType(ComponentType componentType)
        {
            ComponentType = componentType;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="Type"/>.
        /// </summary>
        /// <param name="type">New value of the <see cref="Type"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder WithType(Type type)
        {
            Type = type;
            TypeConverter = Modal._interactionService.GetComponentTypeConverter(type);
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="DefaultValue"/>.
        /// </summary>
        /// <param name="value">New value of the <see cref="DefaultValue"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder SetDefaultValue(object value)
        {
            DefaultValue = value;
            return Instance;
        }

        /// <summary>
        ///     Adds attributes to <see cref="Attributes"/>.
        /// </summary>
        /// <param name="attributes">New attributes to be added to <see cref="Attributes"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder WithAttributes(params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return Instance;
        }

        internal abstract TInfo Build(ModalInfo modal);

        //IInputComponentBuilder
        /// <inheritdoc/>
        IInputComponentBuilder IInputComponentBuilder.WithCustomId(string customId) => WithCustomId(customId);

        /// <inheritdoc/>
        IInputComponentBuilder IInputComponentBuilder.WithLabel(string label) => WithCustomId(label);

        /// <inheritdoc/>
        IInputComponentBuilder IInputComponentBuilder.WithType(Type type) => WithType(type);

        /// <inheritdoc/>
        IInputComponentBuilder IInputComponentBuilder.SetDefaultValue(object value) => SetDefaultValue(value);

        /// <inheritdoc/>
        IInputComponentBuilder IInputComponentBuilder.WithAttributes(params Attribute[] attributes) => WithAttributes(attributes);

        /// <inheritdoc/>
        IInputComponentBuilder IInputComponentBuilder.SetIsRequired(bool isRequired) => SetIsRequired(isRequired);
    }
}
