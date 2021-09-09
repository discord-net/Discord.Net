using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic ratelimit info.
    /// </summary>
    public interface IRateLimitInfo
    {
        /// <summary>
        ///     Gets whether or not this ratelimit info is global.
        /// </summary>
        bool IsGlobal { get; }

        /// <summary>
        ///     Gets the number of requests that can be made.
        /// </summary>
        int? Limit { get; }

        /// <summary>
        ///     Gets the number of remaining requests that can be made.
        /// </summary>
        int? Remaining { get; }

        /// <summary>
        ///     Gets the total time (in seconds) of when the current rate limit bucket will reset. Can have decimals to match previous millisecond ratelimit precision.
        /// </summary>
        int? RetryAfter { get; }

        /// <summary>
        ///     Gets the <see cref="DateTimeOffset"/> at which the rate limit resets.
        /// </summary>
        DateTimeOffset? Reset { get; }

        /// <summary>
        ///     Gets the absolute time when this ratelimit resets.
        /// </summary>
        TimeSpan? ResetAfter { get; }

        /// <summary>
        ///     Gets a unique string denoting the rate limit being encountered (non-inclusive of major parameters in the route path).
        /// </summary>
        string Bucket { get; }

        /// <summary>
        ///     Gets the amount of lag for the request. This is used to denote the precise time of when the ratelimit expires.
        /// </summary>
        TimeSpan? Lag { get; }

        /// <summary>
        ///     Gets the endpoint that this ratelimit info came from.
        /// </summary>
        string Endpoint { get; }
    }
}
