using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Specifies choices for command group.
    /// </summary>
    public interface IApplicationCommandOptionChoice
    {
        /// <summary>
        ///     Gets the choice name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the value of the choice.
        /// </summary>
        object Value { get; }

        /// <summary>
        ///     Gets the localization dictionary for the name field of this command option.
        /// </summary>
        IReadOnlyDictionary<string, string> NameLocalizations { get; }

        /// <summary>
        ///     Gets the localized name of this command option.
        /// </summary>
        /// <remarks>
        ///     Only returned when the `withLocalizations` query parameter is set to <see langword="false"/> when requesting the command.
        /// </remarks>
        string NameLocalized { get; }
    }
}
