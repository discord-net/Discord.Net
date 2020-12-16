using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IApplicationCommandInteractionDataOption
    {
        string Name { get; }
        ApplicationCommandOptionType Value { get; }
        IEnumerable<IApplicationCommandInteractionDataOption> Options { get; }

    }
}
