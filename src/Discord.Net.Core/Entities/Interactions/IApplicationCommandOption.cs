using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// Options for the <see cref="IApplicationCommand"/>, see <see href="https://discord.com/developers/docs/interactions/slash-commands#applicationcommandoption"/>
    /// </summary>
    public interface IApplicationCommandOption
    {
        /// <summary>
        /// The type of this <see cref="IApplicationCommandOption"/>
        /// </summary>
        ApplicationCommandOptionType Type { get; }

        /// <summary>
        /// The name of this command option, 1-32 character name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The discription of this command option, 1-100 character description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// the first required option for the user to complete--only one option can be default
        /// </summary>
        bool? Default { get; }

        /// <summary>
        /// if the parameter is required or optional--default <see langword="false"/>
        /// </summary>
        bool? Required { get; }

        /// <summary>
        /// choices for string and int types for the user to pick from
        /// </summary>
        IEnumerable<IApplicationCommandOptionChoice>? Choices { get; }

        /// <summary>
        /// if the option is a subcommand or subcommand group type, this nested options will be the parameters
        /// </summary>
        IEnumerable<IApplicationCommandOption>? Options { get; }
    }
}
