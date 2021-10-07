using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Options for the <see cref="IApplicationCommand"/>, see <see href="https://discord.com/developers/docs/interactions/slash-commands#applicationcommandoption">The docs</see>.
    /// </summary>
    public interface IApplicationCommandOption
    {
        /// <summary>
        ///     Gets the type of this <see cref="IApplicationCommandOption"/>.
        /// </summary>
        ApplicationCommandOptionType Type { get; }

        /// <summary>
        ///     Gets the name of this command option, 1-32 character name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the discription of this command option, 1-100 character description.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Gets the first required option for the user to complete--only one option can be default.
        /// </summary>
        bool? IsDefault { get; }

        /// <summary>
        ///     Gets if the parameter is required or optional, default is <see langword="false"/>.
        /// </summary>
        bool? IsRequired { get; }

        /// <summary>
        ///     Gets a collection of choices for the user to pick from.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandOptionChoice>? Choices { get; }

        /// <summary>
        ///     Gets the nested options of this option.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandOption>? Options { get; }
    }
}
