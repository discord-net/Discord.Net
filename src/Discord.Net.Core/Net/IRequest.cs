using System;

namespace Discord.Net
{
    /// <summary>
    ///     Represents a generic request to be sent to Discord.
    /// </summary>
    public interface IRequest
    {
        DateTimeOffset? TimeoutAt { get; }
        RequestOptions Options { get; }
    }
}
