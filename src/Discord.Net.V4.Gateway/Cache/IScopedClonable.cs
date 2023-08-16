using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.Cache
{
    public interface IScopedClonable : ICloneable
    {
        void DisposeClone();
    }
}
