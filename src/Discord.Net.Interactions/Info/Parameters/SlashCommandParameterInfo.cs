using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a cached argument constructor delegate.
    /// </summary>
    /// <param name="args">Method arguments array.</param>
    /// <returns>
    ///     Returns the constructed object.
    /// </returns>
    public delegate object ComplexParameterInitializer(object[] args);

    /// <summary>
    ///     Represents the parameter info class for <see cref="SlashCommandInfo"/> commands.
    /// </summary>
    public class SlashCommandParameterInfo : CommandParameterInfo
    {
        internal readonly ComplexParameterInitializer _complexParameterInitializer;

        /// <inheritdoc/>
        public new SlashCommandInfo Command => base.Command as SlashCommandInfo;

        /// <summary>
        ///     Gets the description of the Slash Command Parameter.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Gets the minimum value permitted for a number type parameter.
        /// </summary>
        public double? MinValue { get; }

        /// <summary>
        ///     Gets the maximum value permitted for a number type parameter.
        /// </summary>
        public double? MaxValue { get; }

        /// <summary>
        ///     Gets the minimum length allowed for a string type parameter.
        /// </summary>
        public int? MinLength { get; }

        /// <summary>
        ///     Gets the maximum length allowed for a string type parameter.
        /// </summary>
        public int? MaxLength { get; }

        /// <summary>
        ///     Gets the <see cref="TypeConverter{T}"/> that will be used to convert the incoming <see cref="Discord.IDiscordInteractionData"/> into
        ///     <see cref="CommandParameterInfo.ParameterType"/>.
        /// </summary>
        public TypeConverter TypeConverter { get; }

        /// <summary>
        ///     Gets the <see cref="IAutocompleteHandler"/> that's linked to this parameter.
        /// </summary>
        public IAutocompleteHandler AutocompleteHandler { get; }

        /// <summary>
        ///     Gets whether this parameter is configured for Autocomplete Interactions.
        /// </summary>
        public bool IsAutocomplete { get; }

        /// <summary>
        ///     Gets whether this type should be treated as a complex parameter.
        /// </summary>
        public bool IsComplexParameter { get; }

        /// <summary>
        ///     Gets the Discord option type this parameter represents. If the parameter is not a complex parameter.
        /// </summary>
        public ApplicationCommandOptionType? DiscordOptionType => TypeConverter?.GetDiscordType();

        /// <summary>
        ///     Gets the parameter choices of this Slash Application Command parameter.
        /// </summary>
        public IReadOnlyCollection<ParameterChoice> Choices { get; }

        /// <summary>
        ///     Gets the allowed channel types for this option.
        /// </summary>
        public IReadOnlyCollection<ChannelType> ChannelTypes { get; }

        /// <summary>
        ///     Gets the constructor parameters of this parameter, if <see cref="IsComplexParameter"/> is <see langword="true"/>.
        /// </summary>
        public IReadOnlyCollection<SlashCommandParameterInfo> ComplexParameterFields { get; }

        internal SlashCommandParameterInfo(Builders.SlashCommandParameterBuilder builder, SlashCommandInfo command) : base(builder, command)
        {
            TypeConverter = builder.TypeConverter;
            AutocompleteHandler = builder.AutocompleteHandler;
            Description = builder.Description;
            MaxValue = builder.MaxValue;
            MinValue = builder.MinValue;
            MinLength = builder.MinLength;
            MaxLength = builder.MaxLength;
            IsComplexParameter = builder.IsComplexParameter;
            IsAutocomplete = builder.Autocomplete;
            Choices = builder.Choices.ToImmutableArray();
            ChannelTypes = builder.ChannelTypes.ToImmutableArray();
            ComplexParameterFields = builder.ComplexParameterFields?.Select(x => x.Build(command)).ToImmutableArray();

            _complexParameterInitializer = builder.ComplexParameterInitializer;
        }
    }
}
