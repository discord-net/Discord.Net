using Discord.API;
using Discord.Net.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Discord.Net
{
    /// <summary>
    ///     Represents a REST-Based ratelimit info.
    /// </summary>
    public struct RateLimitInfo : IRateLimitInfo
    {
        /// <inheritdoc/>
        public bool IsGlobal { get; }

        /// <inheritdoc/>
        public int? Limit { get; }

        /// <inheritdoc/>
        public int? Remaining { get; }

        /// <inheritdoc/>
        public int? RetryAfter { get; }

        /// <inheritdoc/>
        public DateTimeOffset? Reset { get; }

        /// <inheritdoc/>
        public TimeSpan? ResetAfter { get; private set; }

        /// <inheritdoc/>
        public string Bucket { get; }

        /// <inheritdoc/>
        public TimeSpan? Lag { get; }

        /// <inheritdoc/>
        public string Endpoint { get; }

        internal RateLimitInfo(Dictionary<string, string> headers, string endpoint)
        {
            Endpoint = endpoint;

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
                double.TryParse(temp, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var resetAfter) ? TimeSpan.FromSeconds(resetAfter) : (TimeSpan?)null;
            Bucket = headers.TryGetValue("X-RateLimit-Bucket", out temp) ? temp : null;
            Lag = headers.TryGetValue("Date", out temp) &&
                DateTimeOffset.TryParse(temp, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? DateTimeOffset.UtcNow - date : (TimeSpan?)null;
        }

        internal Ratelimit ReadRatelimitPayload(Stream response)
        {
            if (response != null && response.Length != 0)
            {
                using (TextReader text = new StreamReader(response))
                using (JsonReader reader = new JsonTextReader(text))
                {
                    return Discord.Rest.DiscordRestClient.Serializer.Deserialize<Ratelimit>(reader);
                }
            }

            return null;
        }
    }
}
