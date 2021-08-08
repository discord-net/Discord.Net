using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     The base command model that belongs to an application. see <see href="https://discord.com/developers/docs/interactions/slash-commands#applicationcommand"/>
    /// </summary>
    public interface IApplicationCommand : ISnowflakeEntity, IDeletable
    {
        /// <summary>
        ///     Gets the unique id of the parent application.
        /// </summary>
        ulong ApplicationId { get; }

        /// <summary>
        ///     The name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The description of the command.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Whether the command is enabled by default when the app is added to a guild.
        /// </summary>
        bool DefaultPermission { get; }

        /// <summary>
        ///     If the option is a subcommand or subcommand group type, this nested options will be the parameters.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandOption> Options { get; }
    }
}
