using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents the base builder class for creating <see cref="CommandInfo{TParameter}"/>.
    /// </summary>
    /// <typeparam name="TInfo">The <see cref="CommandInfo{TParameter}"/> this builder yields when built.</typeparam>
    /// <typeparam name="TBuilder">Inherited <see cref="CommandBuilder{TInfo, TBuilder, TParamBuilder}"/> type.</typeparam>
    /// <typeparam name="TParamBuilder">Builder type for this commands parameters.</typeparam>
    public abstract class CommandBuilder<TInfo, TBuilder, TParamBuilder> : ICommandBuilder
        where TInfo : class, ICommandInfo
        where TBuilder : CommandBuilder<TInfo, TBuilder, TParamBuilder>
        where TParamBuilder : class, IParameterBuilder
    {
        private readonly List<Attribute> _attributes;
        private readonly List<PreconditionAttribute> _preconditions;
        private readonly List<TParamBuilder> _parameters;

        protected abstract TBuilder Instance { get; }

        /// <inheritdoc/>
        public ModuleBuilder Module { get; }

        /// <inheritdoc/>
        public ExecuteCallback Callback { get; internal set; }

        /// <inheritdoc/>
        public string Name { get; internal set; }

        /// <inheritdoc/>
        public string MethodName { get; set; }

        /// <inheritdoc/>
        public bool IgnoreGroupNames { get; set; }

        /// <inheritdoc/>
        public bool TreatNameAsRegex { get; set; }

        /// <inheritdoc/>
        public RunMode RunMode { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<Attribute> Attributes => _attributes;

        /// <inheritdoc/>
        public IReadOnlyList<TParamBuilder> Parameters => _parameters;

        /// <inheritdoc/>
        public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;

        /// <inheritdoc/>
        IReadOnlyList<IParameterBuilder> ICommandBuilder.Parameters => Parameters;

        internal CommandBuilder(ModuleBuilder module)
        {
            _attributes = new List<Attribute>();
            _preconditions = new List<PreconditionAttribute>();
            _parameters = new List<TParamBuilder>();

            Module = module;
        }

        protected CommandBuilder(ModuleBuilder module, string name, ExecuteCallback callback) : this(module)
        {
            Name = name;
            Callback = callback;
        }

        /// <summary>
        ///     Sets <see cref="Name"/>.
        /// </summary>
        /// <param name="name">New value of the <see cref="Name"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder WithName(string name)
        {
            Name = name;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="MethodName"/>.
        /// </summary>
        /// <param name="name">New value of the <see cref="MethodName"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder WithMethodName(string name)
        {
            MethodName = name;
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

        /// <summary>
        ///     Sets <see cref="RunMode"/>.
        /// </summary>
        /// <param name="runMode">New value of the <see cref="RunMode"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder SetRunMode(RunMode runMode)
        {
            RunMode = runMode;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="TreatNameAsRegex"/>.
        /// </summary>
        /// <param name="value">New value of the <see cref="TreatNameAsRegex"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder WithNameAsRegex(bool value)
        {
            TreatNameAsRegex = value;
            return Instance;
        }

        /// <summary>
        ///     Adds parameter builders to <see cref="Parameters"/>.
        /// </summary>
        /// <param name="parameters">New parameter builders to be added to <see cref="Parameters"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder AddParameters(params TParamBuilder[] parameters)
        {
            _parameters.AddRange(parameters);
            return Instance;
        }

        /// <summary>
        ///     Adds preconditions to <see cref="Preconditions"/>.
        /// </summary>
        /// <param name="preconditions">New preconditions to be added to <see cref="Preconditions"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TBuilder WithPreconditions(params PreconditionAttribute[] preconditions)
        {
            _preconditions.AddRange(preconditions);
            return Instance;
        }

        /// <inheritdoc/>
        public abstract TBuilder AddParameter(Action<TParamBuilder> configure);

        internal abstract TInfo Build(ModuleInfo module, InteractionService commandService);

        //ICommandBuilder
        /// <inheritdoc/>
        ICommandBuilder ICommandBuilder.WithName(string name) =>
            WithName(name);

        /// <inheritdoc/>
        ICommandBuilder ICommandBuilder.WithMethodName(string name) =>
            WithMethodName(name);
        ICommandBuilder ICommandBuilder.WithAttributes(params Attribute[] attributes) =>
            WithAttributes(attributes);

        /// <inheritdoc/>
        ICommandBuilder ICommandBuilder.SetRunMode(RunMode runMode) =>
            SetRunMode(runMode);

        /// <inheritdoc/>
        ICommandBuilder ICommandBuilder.WithNameAsRegex(bool value) =>
            WithNameAsRegex(value);

        /// <inheritdoc/>
        ICommandBuilder ICommandBuilder.AddParameters(params IParameterBuilder[] parameters) =>
            AddParameters(parameters as TParamBuilder);

        /// <inheritdoc/>
        ICommandBuilder ICommandBuilder.WithPreconditions(params PreconditionAttribute[] preconditions) =>
            WithPreconditions(preconditions);
    }
}
