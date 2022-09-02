using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents the base builder class for creating <see cref="IParameterInfo"/>.
    /// </summary>
    /// <typeparam name="TInfo">The <see cref="IParameterInfo"/> this builder yields when built.</typeparam>
    /// <typeparam name="TBuilder">Inherited <see cref="ParameterBuilder{TInfo, TBuilder}"/> type.</typeparam>
    public abstract class ParameterBuilder<TInfo, TBuilder> : IParameterBuilder
        where TInfo : class, IParameterInfo
        where TBuilder : ParameterBuilder<TInfo, TBuilder>
    {
        private readonly List<ParameterPreconditionAttribute> _preconditions;
        private readonly List<Attribute> _attributes;

        /// <inheritdoc/>
        public ICommandBuilder Command { get; }

        /// <inheritdoc/>
        public string Name { get; internal set; }

        /// <inheritdoc/>
        public Type ParameterType { get; private set; }

        /// <inheritdoc/>
        public bool IsRequired { get; set; } = true;

        /// <inheritdoc/>
        public bool IsParameterArray { get; set; } = false;

        /// <inheritdoc/>
        public object DefaultValue { get; set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes => _attributes;

        /// <inheritdoc/>
        public IReadOnlyCollection<ParameterPreconditionAttribute> Preconditions => _preconditions;
        protected abstract TBuilder Instance { get; }

        internal ParameterBuilder (ICommandBuilder command)
        {
            _attributes = new List<Attribute>();
            _preconditions = new List<ParameterPreconditionAttribute>();

            Command = command;
        }

        protected ParameterBuilder (ICommandBuilder command, string name, Type type) : this(command)
        {
            Name = name;
            SetParameterType(type);
        }

        /// <summary>
        ///     Sets <see cref="Name"/>.
        /// </summary>
        /// <param name="name">New value of the <see cref="Name"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public virtual TBuilder WithName (string name)
        {
            Name = name;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="ParameterType"/>.
        /// </summary>
        /// <param name="type">New value of the <see cref="ParameterType"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public virtual TBuilder SetParameterType (Type type)
        {
            ParameterType = type;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="IsRequired"/>.
        /// </summary>
        /// <param name="isRequired">New value of the <see cref="IsRequired"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public virtual TBuilder SetRequired (bool isRequired)
        {
            IsRequired = isRequired;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="DefaultValue"/>.
        /// </summary>
        /// <param name="defaultValue">New value of the <see cref="DefaultValue"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public virtual TBuilder SetDefaultValue (object defaultValue)
        {
            DefaultValue = defaultValue;
            return Instance;
        }

        /// <summary>
        ///     Adds attributes to <see cref="Attributes"/>
        /// </summary>
        /// <param name="attributes">New attributes to be added to <see cref="Attributes"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public virtual TBuilder AddAttributes (params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return Instance;
        }

        /// <summary>
        ///     Adds preconditions to <see cref="Preconditions"/>
        /// </summary>
        /// <param name="attributes">New attributes to be added to <see cref="Preconditions"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public virtual TBuilder AddPreconditions (params ParameterPreconditionAttribute[] attributes)
        {
            _preconditions.AddRange(attributes);
            return Instance;
        }

        internal abstract TInfo Build (ICommandInfo command);

        //IParameterBuilder
        /// <inheritdoc/>
        IParameterBuilder IParameterBuilder.WithName (string name) =>
            WithName(name);

        /// <inheritdoc/>
        IParameterBuilder IParameterBuilder.SetParameterType (Type type) =>
            SetParameterType(type);

        /// <inheritdoc/>
        IParameterBuilder IParameterBuilder.SetRequired (bool isRequired) =>
            SetRequired(isRequired);

        /// <inheritdoc/>
        IParameterBuilder IParameterBuilder.SetDefaultValue (object defaultValue) =>
            SetDefaultValue(defaultValue);

        /// <inheritdoc/>
        IParameterBuilder IParameterBuilder.AddAttributes (params Attribute[] attributes) =>
            AddAttributes(attributes);

        /// <inheritdoc/>
        IParameterBuilder IParameterBuilder.AddPreconditions (params ParameterPreconditionAttribute[] preconditions) =>
            AddPreconditions(preconditions);
    }
}
