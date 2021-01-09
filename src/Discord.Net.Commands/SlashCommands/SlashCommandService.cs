using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    public class SlashCommandService
    {
        private List<SlashCommandModule> _modules;

        public SlashCommandService() // TODO: possible config?
        {

        }

        public void AddAssembly()
        {

        }

        public async Task<IResult> ExecuteAsync()
        {
            // TODO: handle execution
            return null;
        }
    }
}
