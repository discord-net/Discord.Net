using System;
using System.Collections.Generic;

namespace Discord.Net
{
    internal struct RateLimitInfo
    {
        public bool IsGlobal { get; }
        public int? Limit { get; }
        public int? Remaining { get; }
        public int? RetryAfter { get; }
        public DateTimeOffset? Reset { get; }
        public TimeSpan? Lag { get; }

        internal RateLimitInfo(Dictionary<string, string> headers)
        {
            string temp;
            IsGlobal = headers.TryGetValue("X-RateLimit-Global", out temp) ? bool.Parse(temp) : false;
            Limit = headers.TryGetValue("X-RateLimit-Limit", out temp) ? int.Parse(temp) : (int?)null;
            Remaining = headers.TryGetValue("X-RateLimit-Remaining", out temp) ? int.Parse(temp) : (int?)null;
            Reset = headers.TryGetValue("X-RateLimit-Reset", out temp) ? 
                DateTimeUtils.FromUnixSeconds(int.Parse(temp)) : (DateTimeOffset?)null;
            RetryAfter = headers.TryGetValue("Retry-After", out temp) ? int.Parse(temp) : (int?)null;
            Lag = headers.TryGetValue("Date", out temp) ? 
                DateTimeOffset.UtcNow - DateTimeOffset.Parse(temp) : (TimeSpan?)null;
        }
    }
}
