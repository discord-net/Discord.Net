using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ApplicationCommandOption;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based implementation of <see cref="IApplicationCommandOption"/>.
    /// </summary>
    public class RestApplicationCommandOption : IApplicationCommandOption
    {
        #region RestApplicationCommandOption
        /// <inheritdoc/>
        public ApplicationCommandOptionType Type { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public bool? IsDefault { get; private set; }

        /// <inheritdoc/>
        public bool? IsRequired { get; private set; }

        /// <inheritdoc/>
        public bool? IsAutocomplete { get; private set; }

        /// <inheritdoc/>
        public double? MinValue { get; private set; }

        /// <inheritdoc/>
        public double? MaxValue { get; private set; }

        /// <summary>
        ///     Gets a collection of <see cref="RestApplicationCommandChoice"/>s for this command.
        /// </summary>
        public IReadOnlyCollection<RestApplicationCommandChoice> Choices { get; private set; }

        /// <summary>
        ///     Gets a collection of <see cref="RestApplicationCommandOption"/>s for this command.
        /// </summary>
        public IReadOnlyCollection<RestApplicationCommandOption> Options { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<ChannelType> ChannelTypes { get; private set; }

        /// <summary>
        ///     Gets the localization dictionary for the name field of this command option.
        /// </summary>
        public IReadOnlyDictionary<string, string> NameLocalizations { get; private set; }

        /// <summary>
        ///     Gets the localization dictionary for the description field of this command option.
        /// </summary>
        public IReadOnlyDictionary<string, string> DescriptionLocalizations { get; private set; }

        /// <summary>
        ///     Gets the localized name of this command option.
        /// </summary>
        /// <remarks>
        ///     Only returned when the `withLocalizations` query parameter is set to <see langword="false"/> when requesting the command.
        /// </remarks>
        public string NameLocalized { get; private set; }

        /// <summary>
        ///     Gets the localized description of this command option.
        /// </summary>
        /// <remarks>
        ///     Only returned when the `withLocalizations` query parameter is set to <see langword="false"/> when requesting the command.
        /// </remarks>
        public string DescriptionLocalized { get; private set; }

        internal RestApplicationCommandOption() { }

        internal static RestApplicationCommandOption Create(Model model)
        {
            var options = new RestApplicationCommandOption();
            options.Update(model);
            return options;
        }

        internal void Update(Model model)
        {
            Type = model.Type;
            Name = model.Name;
            Description = model.Description;

            if (model.Default.IsSpecified)
                IsDefault = model.Default.Value;

            if (model.Required.IsSpecified)
                IsRequired = model.Required.Value;

            if (model.MinValue.IsSpecified)
                MinValue = model.MinValue.Value;

            if (model.MaxValue.IsSpecified)
                MaxValue = model.MaxValue.Value;

            if (model.Autocomplete.IsSpecified)
                IsAutocomplete = model.Autocomplete.Value;

            Options = model.Options.IsSpecified
                ? model.Options.Value.Select(Create).ToImmutableArray()
                : ImmutableArray.Create<RestApplicationCommandOption>();

            Choices = model.Choices.IsSpecified
                ? model.Choices.Value.Select(x => new RestApplicationCommandChoice(x)).ToImmutableArray()
                : ImmutableArray.Create<RestApplicationCommandChoice>();

            ChannelTypes = model.ChannelTypes.IsSpecified
                ? model.ChannelTypes.Value.ToImmutableArray()
                : ImmutableArray.Create<ChannelType>();

            NameLocalizations = model.NameLocalizations.GetValueOrDefault(null)?.ToImmutableDictionary() ??
                                ImmutableDictionary<string, string>.Empty;

            DescriptionLocalizations = model.DescriptionLocalizations.GetValueOrDefault(null)?.ToImmutableDictionary() ??
                                       ImmutableDictionary<string, string>.Empty;

            NameLocalized = model.NameLocalized.GetValueOrDefault();
            DescriptionLocalized = model.DescriptionLocalized.GetValueOrDefault();
        }
        #endregion

        #region IApplicationCommandOption
        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommandOption.Options
            => Options;
        IReadOnlyCollection<IApplicationCommandOptionChoice> IApplicationCommandOption.Choices
            => Choices;
        #endregion
    }
}
