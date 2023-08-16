using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public interface IEventDispatcher<T>
        where T : Delegate
    {
        Task DispatchAsync(T func, params IEntityHandle[] entities);
    }
}
