using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    internal class LegacyEventDispatcher : IEventDispatcher
    {
        public async ValueTask DispatchAsync(Type eventType, EventHandler.Handle[] handles)
        {
            foreach(var handle in handles)
            {
                await handle.RunAsync();
            }
        }
    }
}
