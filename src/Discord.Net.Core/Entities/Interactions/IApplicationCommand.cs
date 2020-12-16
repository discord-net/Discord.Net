using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// The base command model that belongs to an application. see <see href="https://discord.com/developers/docs/interactions/slash-commands#applicationcommand"/>
    /// </summary>
    public interface IApplicationCommand : ISnowflakeEntity
    {
        /// <summary>
        /// Gets the unique id of the command
        /// </summary>
        ulong Id { get; }

        /// <summary>
        /// Gets the unique id of the parent application
        /// </summary>
        ulong ApplicationId { get; }

        /// <summary>
        /// The name of the command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of the command
        /// </summary>
        string Description { get; }

        IEnumerable<IApplicationCommandOption>? Options { get; }
    }
}
