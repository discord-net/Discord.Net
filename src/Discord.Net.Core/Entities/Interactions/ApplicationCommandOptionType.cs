using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// The option type of the Slash command parameter, See <see href="https://discord.com/developers/docs/interactions/slash-commands#applicationcommandoptiontype"/>
    /// </summary>
    public enum ApplicationCommandOptionType : byte
    {
        SubCommand = 1,
        SubCommandGroup = 2,
        String = 3,
        Integer = 4,
        Boolean = 5,
        User = 6,
        Channel = 7,
        Role = 8
    }
}
