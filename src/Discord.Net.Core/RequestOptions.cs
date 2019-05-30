using System.Threading;

namespace Discord
{
    /// <summary>
    ///     Represents options that should be used when sending a request.
    /// </summary>
    public class RequestOptions
    {
        /// <summary>
        ///     Gets or sets the default <see cref="RequestOptions" /> used when not specified with a request.
        /// </summary>
        public static RequestOptions Default { get; set; } = new RequestOptions();

        /// <summary>
        ///     Gets or sets the maximum time to wait for for this request to complete.
        /// </summary>
        /// <remarks>
        ///     Gets or set the max time, in milliseconds, to wait for for this request to complete. If 
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

        internal bool IgnoreState { get; set; }
        internal string BucketId { get; set; }
        internal bool IsClientBucket { get; set; }
        internal bool IsReactionBucket { get; set; }

        internal static RequestOptions CreateOrClone(RequestOptions options)
        {            
            if (options == null)
                return Default.Clone();
            else
                return options.Clone();
        }

        /// <summary>
        ///     Initializes a new <see cref="RequestOptions" /> class with the default request timeout set in
        ///     <see cref="DiscordConfig"/>.
        /// </summary>
        public RequestOptions()
        {
            Timeout = DiscordConfig.DefaultRequestTimeout;
        }
        
        public RequestOptions Clone() => MemberwiseClone() as RequestOptions;
        
        /// <summary>
        ///     Creates a copy of the current <see cref="RequestOptions" /> instance and sets the 
        ///     <see cref="AuditLogReason"/> property.
        /// </summary>
        public RequestOptions WithAuditLogReason (string reason)
        {
            var clone = Clone();
            clone.AuditLogReason = reason;
            return clone;
        }
        
        /// <summary>
        ///     Creates a copy of the current <see cref="RequestOptions" /> instance and sets the 
        ///     <see cref="RetryMode"/> property.
        /// </summary>
        public RequestOptions WithRetryMode (RetryMode retryMode)
        {
            var clone = Clone();
            clone.RetryMode = retryMode;
            return clone;
        }
        
        /// <summary>
        ///     Creates a copy of the current <see cref="RequestOptions" /> instance and sets the 
        ///     <see cref="CancelToken"/> property.
        /// </summary>
        public RequestOptions WithCancellationToken (CancellationToken cancelToken)
        {
            var clone = Clone();
            clone.CancelToken = cancelToken;
            return clone;
        }
        
        /// <summary>
        ///     Creates a copy of the current <see cref="RequestOptions" /> instance and sets the 
        ///     <see cref="Timeout"/> property.
        /// </summary>
        public RequestOptions WithTimeout (int? timeout)
        {
            var clone = Clone();
            clone.Timeout = timeout;
            return clone;
        }
    }
}
