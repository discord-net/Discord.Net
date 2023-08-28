using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public interface IEventDispatcher
    {
        ValueTask DispatchAsync<T>(T[] funcs, object[] args)
            where T : Delegate;
    }
}
