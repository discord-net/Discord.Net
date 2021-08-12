using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public enum ApplicationCommandType : byte
    {
        Slash = 1,
        User = 2,
        Message = 3
    }
}
