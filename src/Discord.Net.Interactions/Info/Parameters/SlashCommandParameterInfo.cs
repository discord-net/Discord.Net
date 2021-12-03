using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the parameter info class for <see cref="SlashCommandInfo"/> commands.
    /// </summary>
    public class SlashCommandParameterInfo : CommandParameterInfo
    {
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
        ///     Gets the maxmimum value permitted for a number type parameter.
        /// </summary>
        public double? MaxValue { get; }

        /// <summary>
        ///     Gets the <see cref="TypeConverter{T}"/> that will be used to convert the incoming <see cref="Discord.WebSocket.SocketSlashCommandDataOption"/> into
        ///     <see cref="CommandParameterInfo.ParameterType"/>.
        /// </summary>
        public TypeConverter TypeConverter { get; }

        /// <summary>
        ///     Gets the <see cref="IAutocompleteHandler"/> thats linked to this parameter.
        /// </summary>
        public IAutocompleteHandler AutocompleteHandler { get; }

        /// <summary>
        ///     Gets whether this parameter is configured for Autocomplete Interactions.
        /// </summary>
        public bool IsAutocomplete { get; }

        /// <summary>
        ///     Gets the Discord option type this parameter represents.
        /// </summary>
        public ApplicationCommandOptionType DiscordOptionType => TypeConverter.GetDiscordType();

        /// <summary>
        ///     Gets the parameter choices of this Slash Application Command parameter.
        /// </summary>
        public IReadOnlyCollection<ParameterChoice> Choices { get; }

        /// <summary>
        ///     Gets the allowed channel types for this option.
        /// </summary>
        public IReadOnlyCollection<ChannelType> ChannelTypes { get; }

        internal SlashCommandParameterInfo(Builders.SlashCommandParameterBuilder builder, SlashCommandInfo command) : base(builder, command)
        {
            TypeConverter = builder.TypeConverter;
            AutocompleteHandler = builder.AutocompleteHandler;
            Description = builder.Description;
            MaxValue = builder.MaxValue;
            MinValue = builder.MinValue;
            IsAutocomplete = builder.Autocomplete;
            Choices = builder.Choices.ToImmutableArray();
            ChannelTypes = builder.ChannelTypes.ToImmutableArray();
        }
    }
}
