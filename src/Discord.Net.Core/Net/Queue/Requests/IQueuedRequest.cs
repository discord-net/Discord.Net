using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public interface IQueuedRequest
    {
        CancellationToken CancelToken { get; }
        int? TimeoutTick { get; }

        Task<Stream> SendAsync();
    }
}
