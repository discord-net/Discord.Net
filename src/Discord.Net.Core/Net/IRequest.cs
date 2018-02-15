using System;
using System.IO;
using System.Threading.Tasks;

namespace Discord.Net
{
    public interface IRequest
    {
        DateTimeOffset? TimeoutAt { get; }
        TaskCompletionSource<Stream> Promise { get; }
        RequestOptions Options { get; }
        
    }
}
