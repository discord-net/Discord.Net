using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface ISocketMessageChannel : IMessageChannel
    {
        IAsyncEnumerable<SocketMessage> GetCachedMessages(CancellationToken token = default);


    }
}
