using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    internal class LegacyEventDispatcher : IEventDispatcher
    {
        public async ValueTask DispatchAsync<T>(T[] funcs, object[] args)
            where T : Delegate

        {
            foreach(var func in funcs)
            {
                var result = func.DynamicInvoke(args);

                if (result is Task task)
                {
                    await task;
                }
                else if (result is ValueTask vt)
                {
                    await vt;
                }
            }
        }
    }
}
