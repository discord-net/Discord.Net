using System.Collections.Generic;

namespace Discord.Rpc
{
    public interface IRpcMessageChannel : IMessageChannel
    {
        IReadOnlyCollection<RpcMessage> CachedMessages { get; }
    }
}
