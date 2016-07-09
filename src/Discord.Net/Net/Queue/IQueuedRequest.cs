using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    //TODO: Allow user-supplied canceltoken
    //TODO: Allow specifying timeout via DiscordApiClient
    internal interface IQueuedRequest
    {
        CancellationToken CancelToken { get; }
        int? TimeoutTick { get; }

        Task<Stream> SendAsync();
    }
}
