using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

internal sealed class LegacyEventDispatcher : IEventDispatcher
{
    public static readonly LegacyEventDispatcher Instance = new();

    public async ValueTask DispatchAsync(
        string eventName,
        IEnumerable<PreparedInvocableEventHandle> handlers,
        CancellationToken token)
    {
        foreach (var handler in handlers)
        {
            await handler(token);
        }
    }
}
