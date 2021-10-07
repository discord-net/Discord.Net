using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents data of an Interaction Command, see <see href="https://discord.com/developers/docs/interactions/slash-commands#interaction-applicationcommandinteractiondata"/>.
    /// </summary>
    public interface IApplicationCommandInteractionData
    {
        /// <summary>
        ///     Gets the snowflake id of this command.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        ///     Gets the name of this command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the options that the user has provided.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; }
    }
}
