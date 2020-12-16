using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IApplicationCommandInteractionData
    {
        ulong Id { get; }
        string Name { get; }
        IEnumerable<IApplicationCommandInteractionDataOption> Options { get; }
    }
}
