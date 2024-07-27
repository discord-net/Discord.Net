using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.State;

public enum EntityHandleFlags
{
    None = 0,
    UpdateOnRelease = 1 << 0,
    DeleteOnRelease = 1 << 1

}
