using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    public class SlashModuleInfo
    {
        public SlashModuleInfo(SlashCommandService service)
        {
            Service = service;
        }

        /// <summary>
        ///     Gets the command service associated with this module.
        /// </summary>
        public SlashCommandService Service { get; }
        /// <summary>
        ///     Gets a read-only list of commands associated with this module.
        /// </summary>
        public List<SlashCommandInfo> Commands { get; private set; }

        /// <summary>
        /// The user command module defined as the interface ISlashCommandModule
        /// Used to set context.
        /// </summary>
        public ISlashCommandModule userCommandModule;


        public void SetCommands(List<SlashCommandInfo> commands)
        {
            if (this.Commands == null)
            {
                this.Commands = commands;
            }
        }
        public void SetCommandModule(object userCommandModule)
        {
            if (this.userCommandModule == null)
            {
                this.userCommandModule = userCommandModule as ISlashCommandModule;
            }
        }
    }
}
