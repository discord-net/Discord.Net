using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a option group for a command.
    /// </summary>
    public interface IApplicationCommandInteractionDataOption
    {
        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the value of the pair.
        ///     <note>
        ///         This objects type can be any one of the option types in <see cref="ApplicationCommandOptionType"/>.
        ///     </note>
        /// </summary>
        object Value { get; }

        /// <summary>
        ///     Gets the type of this data's option.
        /// </summary>
        ApplicationCommandOptionType Type { get; }

        /// <summary>
        ///     Gets the nested options of this option.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; }
    }
}
