using System;
using System.Collections.Generic;
using System.Reflection;

namespace Discord.Interactions.Builders;

public interface IModalComponentBuilder
{
    /// <summary>
    ///     Gets the parent modal of this input component.
    /// </summary>
    ModalBuilder Modal { get; }

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
    ///     Gets the default value of this input component property.
    /// </summary>
    object DefaultValue { get; }

    /// <summary>
    ///     Gets a collection of the attributes of this component.
    /// </summary>
    IReadOnlyCollection<Attribute> Attributes { get; }

    /// <summary>
    ///     Sets <see cref="Type"/>.
    /// </summary>
    /// <param name="type">New value of the <see cref="Type"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    IModalComponentBuilder WithType(Type type);

    /// <summary>
    ///     Sets <see cref="DefaultValue"/>.
    /// </summary>
    /// <param name="value">New value of the <see cref="DefaultValue"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    IModalComponentBuilder SetDefaultValue(object value);

    /// <summary>
    ///     Adds attributes to <see cref="Attributes"/>.
    /// </summary>
    /// <param name="attributes">New attributes to be added to <see cref="Attributes"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    IModalComponentBuilder WithAttributes(params Attribute[] attributes);
}
