using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public interface IEventDispatcher
    {
        ValueTask DispatchAsync(Type eventType, EventHandler.Handle[] handles);
    }
}
