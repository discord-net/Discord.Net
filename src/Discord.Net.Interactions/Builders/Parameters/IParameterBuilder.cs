using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represent a command builder for creating <see cref="IParameterInfo"/>.
    /// </summary>
    public interface IParameterBuilder
    {
        /// <summary>
        ///     Gets the parent command of this parameter.
        /// </summary>
        ICommandBuilder Command { get; }

        /// <summary>
        ///     Gets the name of this parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the type of this parameter.
        /// </summary>
        Type ParameterType { get; }

        /// <summary>
        ///     Gets whether this parameter is required.
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        ///     Gets whether this parameter is <see langword="params"/>.
        /// </summary>
        bool IsParameterArray { get; }

        /// <summary>
        ///     Gets the default value of this parameter.
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        ///     Gets a collection of the attributes of this command.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     Gets a collection of the preconditions of this command.
        /// </summary>
        IReadOnlyCollection<ParameterPreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     Sets <see cref="Name"/>.
        /// </summary>
        /// <param name="name">New value of the <see cref="Name"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IParameterBuilder WithName(string name);

        /// <summary>
        ///     Sets <see cref="ParameterType"/>.
        /// </summary>
        /// <param name="type">New value of the <see cref="ParameterType"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IParameterBuilder SetParameterType(Type type);

        /// <summary>
        ///     Sets <see cref="IsRequired"/>.
        /// </summary>
        /// <param name="isRequired">New value of the <see cref="IsRequired"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IParameterBuilder SetRequired(bool isRequired);

        /// <summary>
        ///     Sets <see cref="DefaultValue"/>.
        /// </summary>
        /// <param name="defaultValue">New value of the <see cref="DefaultValue"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IParameterBuilder SetDefaultValue(object defaultValue);

        /// <summary>
        ///     Adds attributes to <see cref="Attributes"/>.
        /// </summary>
        /// <param name="attributes">New attributes to be added to <see cref="Attributes"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IParameterBuilder AddAttributes(params Attribute[] attributes);

        /// <summary>
        ///     Adds preconditions to <see cref="Preconditions"/>.
        /// </summary>
        /// <param name="preconditions">New attributes to be added to <see cref="Preconditions"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        IParameterBuilder AddPreconditions(params ParameterPreconditionAttribute[] preconditions);
    }
}
