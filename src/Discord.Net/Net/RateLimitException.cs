using System.Net;

namespace Discord.Net
{
    public class HttpRateLimitException : HttpException
    {
        public string BucketId { get; }
        public int RetryAfterMilliseconds { get; }

        public HttpRateLimitException(string bucketId, int retryAfterMilliseconds, string reason)
            : base((HttpStatusCode)429, reason)
        {
            BucketId = bucketId;
            RetryAfterMilliseconds = retryAfterMilliseconds;
        }
    }
}
