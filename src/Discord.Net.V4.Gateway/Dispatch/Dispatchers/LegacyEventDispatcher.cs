using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

internal sealed class LegacyEventDispatcher : IEventDispatcher
{
    public async ValueTask DispatchAsync<T>(string eventName, HashSet<T> handlers, CancellationToken token)
        where T : ITransientDispatchHandler
    {
        token.ThrowIfCancellationRequested();

        foreach (var handler in handlers)
        {
            await handler.ExecuteAsync(token);

            token.ThrowIfCancellationRequested();
        }
    }
}
