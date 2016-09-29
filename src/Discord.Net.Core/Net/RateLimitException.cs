using System.Net;

namespace Discord.Net
{
    public class HttpRateLimitException : HttpException
    {
        public string Id { get; }
        public int RetryAfterMilliseconds { get; }

        public HttpRateLimitException(string bucketId, int retryAfterMilliseconds, string reason)
            : base((HttpStatusCode)429, reason)
        {
            Id = bucketId;
            RetryAfterMilliseconds = retryAfterMilliseconds;
        }
    }
}
