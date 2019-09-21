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
		    public TimeSpan? ResetAfter { get; }
        public TimeSpan? Lag { get; }

        internal RateLimitInfo(Dictionary<string, string> headers)
        {
            IsGlobal = headers.TryGetValue("X-RateLimit-Global", out string temp) &&
                       bool.TryParse(temp, out var isGlobal) && isGlobal;
            Limit = headers.TryGetValue("X-RateLimit-Limit", out temp) && 
                int.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out var limit) ? limit : (int?)null;
            Remaining = headers.TryGetValue("X-RateLimit-Remaining", out temp) && 
                int.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out var remaining) ? remaining : (int?)null;
            Reset = headers.TryGetValue("X-RateLimit-Reset", out temp) && 
                double.TryParse(temp, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var reset) ? DateTimeOffset.FromUnixTimeMilliseconds((long)(reset * 1000)) : (DateTimeOffset?)null;
            RetryAfter = headers.TryGetValue("Retry-After", out temp) &&
                int.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out var retryAfter) ? retryAfter : (int?)null;
			      ResetAfter = headers.TryGetValue("X-RateLimit-Reset-After", out temp) && 
				        float.TryParse(temp, out var resetAfter) ? TimeSpan.FromMilliseconds((long)(resetAfter * 1000)) : (TimeSpan?)null;
            Lag = headers.TryGetValue("Date", out temp) &&
                DateTimeOffset.TryParse(temp, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? DateTimeOffset.UtcNow - date : (TimeSpan?)null;
        }
    }
}
