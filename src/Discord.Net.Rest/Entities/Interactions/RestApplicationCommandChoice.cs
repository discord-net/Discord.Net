using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandOptionChoice;

namespace Discord.Rest
{
    public class RestApplicationCommandChoice : IApplicationCommandOptionChoice
    {
        public string Name { get; }

        public object Value { get; }

        internal RestApplicationCommandChoice(Model model)
        {
            this.Name = model.Name;
            this.Value = model.Value;
        }
    }
}
