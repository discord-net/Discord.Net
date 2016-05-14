using System.Net;

namespace Discord.Net
{
    public class HttpRateLimitException : HttpException
    {
        public int RetryAfterMilliseconds { get; }

        public HttpRateLimitException(int retryAfterMilliseconds)
            : base((HttpStatusCode)429)
        {
            RetryAfterMilliseconds = retryAfterMilliseconds;
        }
    }
}
