using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    ///     Defines the current class or function as a slash command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SlashCommand : Attribute
    {
        /// <summary>
        ///     The name of this slash command.
        /// </summary>
        public string CommandName;

        /// <summary>
        ///     Tells the <see cref="SlashCommandService"/> that this class/function is a slash command.
        /// </summary>
        /// <param name="CommandName">The name of this slash command.</param>
        public SlashCommand(string CommandName)
        {
            this.CommandName = CommandName;
        }
    }
}
