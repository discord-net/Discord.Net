using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="SlashCommandParameterInfo"/>.
    /// </summary>
    public sealed class SlashCommandParameterBuilder : ParameterBuilder<SlashCommandParameterInfo, SlashCommandParameterBuilder>
    {
        private readonly List<ParameterChoice> _choices = new();
        private readonly List<ChannelType> _channelTypes = new();
        private readonly List<SlashCommandParameterBuilder> _complexParameterFields = new();

        /// <summary>
        ///     Gets or sets the description of this parameter.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the max value of this parameter.
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        ///     Gets or sets the min value of this parameter.
        /// </summary>
        public double? MinValue { get; set; }

        /// <summary>
        ///     Gets or sets the minimum length allowed for a string type parameter.
        /// </summary>
        public int? MinLength { get; set; }

        /// <summary>
        ///     Gets or sets the maximum length allowed for a string type parameter.
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        ///     Gets a collection of the choices of this command.
        /// </summary>
        public IReadOnlyCollection<ParameterChoice> Choices => _choices;

        /// <summary>
        ///     Gets a collection of the channel types of this command.
        /// </summary>
        public IReadOnlyCollection<ChannelType> ChannelTypes => _channelTypes;

        /// <summary>
        ///     Gets the constructor parameters of this parameter, if <see cref="IsComplexParameter"/> is <see langword="true"/>.
        /// </summary>
        public IReadOnlyCollection<SlashCommandParameterBuilder> ComplexParameterFields => _complexParameterFields;

        /// <summary>
        ///     Gets or sets whether this parameter should be configured for Autocomplete Interactions.
        /// </summary>
        public bool Autocomplete { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="TypeConverter"/> of this parameter.
        /// </summary>
        public TypeConverter TypeConverter { get; private set; }

        /// <summary>
        ///     Gets whether this type should be treated as a complex parameter.
        /// </summary>
        public bool IsComplexParameter { get; internal set; }

        /// <summary>
        ///     Gets the initializer delegate for this parameter, if <see cref="IsComplexParameter"/> is <see langword="true"/>.
        /// </summary>
        public ComplexParameterInitializer ComplexParameterInitializer { get; internal set; }

        /// <summary>
        ///     Gets or sets the <see cref="IAutocompleteHandler"/> of this parameter.
        /// </summary>
        public IAutocompleteHandler AutocompleteHandler { get; set; }
        protected override SlashCommandParameterBuilder Instance => this;

        internal SlashCommandParameterBuilder(ICommandBuilder command) : base(command) { }

        /// <summary>
        ///     Initializes a new <see cref="SlashCommandParameterBuilder"/>.
        /// </summary>
        /// <param name="command">Parent command of this parameter.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="type">Type of this parameter.</param>
        public SlashCommandParameterBuilder(ICommandBuilder command, string name, Type type, ComplexParameterInitializer complexParameterInitializer = null)
            : base(command, name, type)
        {
            ComplexParameterInitializer = complexParameterInitializer;

            if (complexParameterInitializer is not null)
                IsComplexParameter = true;
        }

        /// <summary>
        ///     Sets <see cref="Description"/>.
        /// </summary>
        /// <param name="description">New value of the <see cref="Description"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandParameterBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="MinValue"/>.
        /// </summary>
        /// <param name="value">New value of the <see cref="MinValue"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandParameterBuilder WithMinValue(double value)
        {
            MinValue = value;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="MaxValue"/>.
        /// </summary>
        /// <param name="value">New value of the <see cref="MaxValue"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandParameterBuilder WithMaxValue(double value)
        {
            MaxValue = value;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="MinLength"/>.
        /// </summary>
        /// <param name="length">New value of the <see cref="MinLength"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandParameterBuilder WithMinLength(int length)
        {
            MinLength = length;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="MaxLength"/>.
        /// </summary>
        /// <param name="length">New value of the <see cref="MaxLength"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandParameterBuilder WithMaxLength(int length)
        {
            MaxLength = length;
            return this;
        }

        /// <summary>
        ///     Adds parameter choices to <see cref="Choices"/>.
        /// </summary>
        /// <param name="options">New choices to be added to <see cref="Choices"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandParameterBuilder WithChoices(params ParameterChoice[] options)
        {
            _choices.AddRange(options);
            return this;
        }

        /// <summary>
        ///     Adds channel types to <see cref="ChannelTypes"/>.
        /// </summary>
        /// <param name="channelTypes">New channel types to be added to <see cref="ChannelTypes"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandParameterBuilder WithChannelTypes(params ChannelType[] channelTypes)
        {
            _channelTypes.AddRange(channelTypes);
            return this;
        }

        /// <summary>
        ///     Adds channel types to <see cref="ChannelTypes"/>.
        /// </summary>
        /// <param name="channelTypes">New channel types to be added to <see cref="ChannelTypes"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandParameterBuilder WithChannelTypes(IEnumerable<ChannelType> channelTypes)
        {
            _channelTypes.AddRange(channelTypes);
            return this;
        }

        /// <summary>
        ///     Sets <see cref="AutocompleteHandler"/>.
        /// </summary>
        /// <param name="autocompleteHandlerType">Type of the <see cref="IAutocompleteHandler"/>.</param>
        /// <param name="services">Service container to be used to resolve the dependencies of this parameters <see cref="TypeConverter"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandParameterBuilder WithAutocompleteHandler(Type autocompleteHandlerType, IServiceProvider services = null)
        {
            AutocompleteHandler = Command.Module.InteractionService.GetAutocompleteHandler(autocompleteHandlerType, services);
            return this;
        }

        /// <inheritdoc/>
        public override SlashCommandParameterBuilder SetParameterType(Type type) => SetParameterType(type, null);

        /// <summary>
        ///     Sets <see cref="ParameterBuilder{TInfo, TBuilder}.ParameterType"/>.
        /// </summary>
        /// <param name="type">New value of the <see cref="ParameterBuilder{TInfo, TBuilder}.ParameterType"/>.</param>
        /// <param name="services">Service container to be used to resolve the dependencies of this parameters <see cref="TypeConverter"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandParameterBuilder SetParameterType(Type type, IServiceProvider services = null)
        {
            base.SetParameterType(type);

            if (!IsComplexParameter)
                TypeConverter = Command.Module.InteractionService.GetTypeConverter(ParameterType, services);

            return this;
        }

        /// <summary>
        ///     Adds a parameter builders to <see cref="ComplexParameterFields"/>.
        /// </summary>
        /// <param name="configure"><see cref="SlashCommandParameterBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the added field has a <see cref="ComplexParameterAttribute"/>.</exception>
        public SlashCommandParameterBuilder AddComplexParameterField(Action<SlashCommandParameterBuilder> configure)
        {
            SlashCommandParameterBuilder builder = new(Command);
            configure(builder);

            if (builder.IsComplexParameter)
                throw new InvalidOperationException("You cannot create nested complex parameters.");

            _complexParameterFields.Add(builder);
            return this;
        }

        /// <summary>
        ///     Adds parameter builders to <see cref="ComplexParameterFields"/>.
        /// </summary>
        /// <param name="fields">New parameter builders to be added to <see cref="ComplexParameterFields"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the added field has a <see cref="ComplexParameterAttribute"/>.</exception>
        public SlashCommandParameterBuilder AddComplexParameterFields(params SlashCommandParameterBuilder[] fields)
        {
            if (fields.Any(x => x.IsComplexParameter))
                throw new InvalidOperationException("You cannot create nested complex parameters.");

            _complexParameterFields.AddRange(fields);
            return this;
        }

        internal override SlashCommandParameterInfo Build(ICommandInfo command) =>
            new SlashCommandParameterInfo(this, command as SlashCommandInfo);
    }
}
