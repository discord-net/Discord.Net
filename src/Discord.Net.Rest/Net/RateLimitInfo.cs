using System;
using System.Collections.Generic;
using System.Globalization;

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
            IsGlobal = headers.TryGetValue("X-RateLimit-Global", out string temp) &&
                       bool.TryParse(temp, out var isGlobal) && isGlobal;
            Limit = headers.TryGetValue("X-RateLimit-Limit", out temp) && 
                int.TryParse(temp, out var limit) ? limit : (int?)null;
            Remaining = headers.TryGetValue("X-RateLimit-Remaining", out temp) && 
                int.TryParse(temp, out var remaining) ? remaining : (int?)null;
            Reset = headers.TryGetValue("X-RateLimit-Reset", out temp) && 
                float.TryParse(temp, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out var reset) ? DateTimeOffset.FromUnixTimeMilliseconds((long)(reset * 1000)) : (DateTimeOffset?)null;
            RetryAfter = headers.TryGetValue("Retry-After", out temp) &&
                int.TryParse(temp, out var retryAfter) ? retryAfter : (int?)null;
            Lag = headers.TryGetValue("Date", out temp) &&
                DateTimeOffset.TryParse(temp, out var date) ? DateTimeOffset.UtcNow - date : (TimeSpan?)null;
        }
    }
}
