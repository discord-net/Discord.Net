using System;

namespace Discord.Net
{
    public class RateLimitedException : TimeoutException
    {
        public RateLimitedException(IRequest request)
            : base("You are being rate limited.")
        {
            Request = request;
        }

        public IRequest Request { get; }
    }
}
