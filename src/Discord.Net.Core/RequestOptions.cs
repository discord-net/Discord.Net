using Discord.Net;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents options that should be used when sending a request.
    /// </summary>
    public class RequestOptions
    {
        /// <summary>
        ///     Creates a new <see cref="RequestOptions" /> class with its default settings.
        /// </summary>
        public static RequestOptions Default => new RequestOptions();

        /// <summary>
        ///     Gets or sets the maximum time to wait for this request to complete.
        /// </summary>
        /// <remarks>
        ///     Gets or set the max time, in milliseconds, to wait for this request to complete. If
        ///     <c>null</c>, a request will not time out. If a rate limit has been triggered for this request's bucket
        ///     and will not be unpaused in time, this request will fail immediately.
        /// </remarks>
        /// <returns>
        ///     A <see cref="int"/> in milliseconds for when the request times out.
        /// </returns>
        public int? Timeout { get; set; }
        /// <summary>
        ///     Gets or sets the cancellation token for this request.
        /// </summary>
        /// <returns>
        ///     A <see cref="CancellationToken"/> for this request.
        /// </returns>
        public CancellationToken CancelToken { get; set; } = CancellationToken.None;
        /// <summary>
        ///     Gets or sets the retry behavior when the request fails.
        /// </summary>
        public RetryMode? RetryMode { get; set; }
        public bool HeaderOnly { get; internal set; }
        /// <summary>
        ///     Gets or sets the reason for this action in the guild's audit log.
        /// </summary>
        /// <remarks>
        ///     Gets or sets the reason that will be written to the guild's audit log if applicable. This may not apply
        ///     to all actions.
        /// </remarks>
        public string AuditLogReason { get; set; }
        /// <summary>
        ///		Gets or sets whether or not this request should use the system
        ///		clock for rate-limiting. Defaults to <c>true</c>.
        /// </summary>
        /// <remarks>
        ///		This property can also be set in <see cref="DiscordConfig"/>.
        ///		On a per-request basis, the system clock should only be disabled
        ///		when millisecond precision is especially important, and the
        ///		hosting system is known to have a desynced clock.
        /// </remarks>
        public bool? UseSystemClock { get; set; }

        /// <summary>
        ///     Gets or sets the callback to execute regarding ratelimits for this request.
        /// </summary>
        public Func<IRateLimitInfo, Task> RatelimitCallback { get; set; }

        internal bool IgnoreState { get; set; }
        internal BucketId BucketId { get; set; }
        internal bool IsClientBucket { get; set; }
        internal bool IsReactionBucket { get; set; }
        internal bool IsGatewayBucket { get; set; }

        internal IDictionary<string, IEnumerable<string>> RequestHeaders { get; }

        internal static RequestOptions CreateOrClone(RequestOptions options)
        {
            if (options == null)
                return new RequestOptions();
            else
                return options.Clone();
        }

        internal void ExecuteRatelimitCallback(IRateLimitInfo info)
        {
            if (RatelimitCallback != null)
            {
                _ = Task.Run(async () =>
                {
                    await RatelimitCallback(info);
                });
            }
        }

        /// <summary>
        ///     Initializes a new <see cref="RequestOptions" /> class with the default request timeout set in
        ///     <see cref="DiscordConfig"/>.
        /// </summary>
        public RequestOptions()
        {
            Timeout = DiscordConfig.DefaultRequestTimeout;
            RequestHeaders = new Dictionary<string, IEnumerable<string>>();
        }

        public RequestOptions Clone() => MemberwiseClone() as RequestOptions;
    }
}
