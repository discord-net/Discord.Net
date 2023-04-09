using System;
using System.Collections.Generic;
using System.Reflection;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represent a builder for creating <see cref="InputComponentInfo"/>.
    /// </summary>
    public interface IInputComponentBuilder
    {
        /// <summary>
        ///     Gets the parent modal of this input component.
        /// </summary>
        ModalBuilder Modal { get; }

        /// <summary>
        ///     Gets the custom id of this input component.
        /// </summary>
        string CustomId { get; }

        /// <summary>
        ///     Gets the label of this input component.
        /// </summary>
        string Label { get; }

        /// <summary>
        ///     Gets whether this input component is required.
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        ///     Gets the component type of this input component.
        /// </summary>
        ComponentType ComponentType { get; }

        /// <summary>
        ///     Get the reference type of this input component.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     Get the <see cref="PropertyInfo"/> of this component's property.
        /// </summary>
        PropertyInfo PropertyInfo { get; }

        /// <summary>
        ///     Get the <see cref="ComponentTypeConverter"/> assigned to this input.
        /// </summary>
        ComponentTypeConverter TypeConverter { get; }

        /// <summary>
        ///     Gets the default value of this input component.
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        ///     Gets a collection of the attributes of this component.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

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
        ///     Sets <see cref="IsRequired"/>.
        /// </summary>
        /// <param name="isRequired">New value of the <see cref="IsRequired"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IInputComponentBuilder SetIsRequired(bool isRequired);

        /// <summary>
        ///     Sets <see cref="Type"/>.
        /// </summary>
        /// <param name="type">New value of the <see cref="Type"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IInputComponentBuilder WithType(Type type);

        /// <summary>
        ///     Sets <see cref="DefaultValue"/>.
        /// </summary>
        /// <param name="value">New value of the <see cref="DefaultValue"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IInputComponentBuilder SetDefaultValue(object value);

        /// <summary>
        ///     Adds attributes to <see cref="Attributes"/>.
        /// </summary>
        /// <param name="attributes">New attributes to be added to <see cref="Attributes"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IInputComponentBuilder WithAttributes(params Attribute[] attributes);
    }
}
