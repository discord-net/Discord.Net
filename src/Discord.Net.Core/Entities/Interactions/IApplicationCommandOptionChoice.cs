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

        IReadOnlyDictionary<string, string>? NameLocalizations { get; }

        string? NameLocalized { get; }
    }
}
