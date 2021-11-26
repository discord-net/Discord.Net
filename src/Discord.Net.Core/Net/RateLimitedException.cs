using System;

namespace Discord.Net
{
    /// <summary>
    ///     The exception that is thrown when the user is being rate limited by Discord.
    /// </summary>
    public class RateLimitedException : TimeoutException
    {
        /// <summary>
        ///     Gets the request object used to send the request.
        /// </summary>
        public IRequest Request { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RateLimitedException" /> class using the
        ///     <paramref name="request"/> sent.
        /// </summary>
        public RateLimitedException(IRequest request)
            : base("You are being rate limited.")
        {
            Request = request;
        }
    }
}
