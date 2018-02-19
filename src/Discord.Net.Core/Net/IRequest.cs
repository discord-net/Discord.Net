using System;

namespace Discord.Net
{
    public interface IRequest
    {
        DateTimeOffset? TimeoutAt { get; }
        RequestOptions Options { get; }
    }
}
