using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="SlashCommandParameterInfo"/>.
    /// </summary>
    public sealed class SlashCommandParameterBuilder : ParameterBuilder<SlashCommandParameterInfo, SlashCommandParameterBuilder>
    {
        private readonly List<ParameterChoice> _choices = new();
        private readonly List<ChannelType> _channelTypes = new();

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
        ///     Gets a collection of the choices of this command.
        /// </summary>
        public IReadOnlyCollection<ParameterChoice> Choices => _choices;

        /// <summary>
        ///     Gets a collection of the channel types of this command.
        /// </summary>
        public IReadOnlyCollection<ChannelType> ChannelTypes => _channelTypes;

        /// <summary>
        ///     Gets or sets whether this parameter should be configured for Autocomplete Interactions.
        /// </summary>
        public bool Autocomplete { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="TypeConverter"/> of this parameter.
        /// </summary>
        public TypeConverter TypeConverter { get; private set; }

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
        public SlashCommandParameterBuilder(ICommandBuilder command, string name, Type type) : base(command, name, type) { }

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
            TypeConverter = Command.Module.InteractionService.GetTypeConverter(ParameterType, services);
            return this;
        }

        internal override SlashCommandParameterInfo Build(ICommandInfo command) =>
            new SlashCommandParameterInfo(this, command as SlashCommandInfo);
    }
}
