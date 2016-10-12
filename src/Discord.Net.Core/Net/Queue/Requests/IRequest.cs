using System;
using System.Threading;

namespace Discord.Net.Queue
{
    public interface IRequest
    {
        CancellationToken CancelToken { get; }
        DateTimeOffset? TimeoutAt { get; }
        string BucketId { get; }
    }
}
