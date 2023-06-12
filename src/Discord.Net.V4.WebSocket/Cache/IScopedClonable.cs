using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    internal interface IScopedClonable : ICloneable
    {
        void DisposeClone();
    }
}
