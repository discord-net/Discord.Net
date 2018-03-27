using System;

namespace Discord.Net
{
    public class RateLimitedException : TimeoutException
    {
        public IRequest Request { get; }

        public RateLimitedException(IRequest request)
            : base("You are being rate limited.")
        {
            Request = request;
        }
    }
}
