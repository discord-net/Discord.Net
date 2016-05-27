using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    internal interface IQueuedRequest
    {
        TaskCompletionSource<Stream> Promise { get; }
        CancellationToken CancelToken { get; }
        Task<Stream> Send();
    }
}
