using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal interface IResolvable
    {
        Optional<ApplicationCommandInteractionDataResolved> Resolved { get; }
    }
}
