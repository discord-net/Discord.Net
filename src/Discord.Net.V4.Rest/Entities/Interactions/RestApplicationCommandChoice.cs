using System.Collections.Generic;
using System.Collections.Immutable;
using Model = Discord.API.ApplicationCommandOptionChoice;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based implementation of <see cref="IApplicationCommandOptionChoice"/>.
    /// </summary>
    public class RestApplicationCommandChoice : IApplicationCommandOptionChoice
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public object Value { get; }

        /// <summary>
        ///     Gets the localization dictionary for the name field of this command option choice.
        /// </summary>
        public IReadOnlyDictionary<string, string> NameLocalizations { get; }

        /// <summary>
        ///     Gets the localized name of this command option choice.
        /// </summary>
        /// <remarks>
        ///     Only returned when the `withLocalizations` query parameter is set to <see langword="false"/> when requesting the command.
        /// </remarks>
        public string NameLocalized { get; }

        internal RestApplicationCommandChoice(Model model)
        {
            Name = model.Name;
            Value = model.Value;
            NameLocalizations = model.NameLocalizations.GetValueOrDefault(null)?.ToImmutableDictionary();
            NameLocalized = model.NameLocalized.GetValueOrDefault(null);
        }
    }
}
