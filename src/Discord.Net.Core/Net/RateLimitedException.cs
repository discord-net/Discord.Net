using System;

namespace Discord.Net
{
    public class RateLimitedException : TimeoutException
    {
        public RateLimitedException()
            : base("You are being rate limited.")
        {
        }
    }
}
