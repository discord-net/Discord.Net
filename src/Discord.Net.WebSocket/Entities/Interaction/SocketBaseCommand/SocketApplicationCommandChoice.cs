using System.Collections.Generic;
using System.Collections.Immutable;
using Model = Discord.API.ApplicationCommandOptionChoice;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a choice for a <see cref="SocketApplicationCommandOption"/>.
    /// </summary>
    public class SocketApplicationCommandChoice : IApplicationCommandOptionChoice
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public object Value { get; private set; }

        /// <summary>
        ///     Gets the localization dictionary for the name field of this command option choice.
        /// </summary>
        public IReadOnlyDictionary<string, string> NameLocalizations { get; private set; }

        /// <summary>
        ///     Gets the localized name of this command option choice.
        /// </summary>
        /// <remarks>
        ///     Only returned when the `withLocalizations` query parameter is set to <see langword="false"/> when requesting the command.
        /// </remarks>
        public string NameLocalized { get; private set; }

        internal SocketApplicationCommandChoice() { }
        internal static SocketApplicationCommandChoice Create(Model model)
        {
            var entity = new SocketApplicationCommandChoice();
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Name = model.Name;
            Value = model.Value;
            NameLocalizations = model.NameLocalizations.GetValueOrDefault(null)?.ToImmutableDictionary();
            NameLocalized = model.NameLocalized.GetValueOrDefault(null);
        }
    }
}
