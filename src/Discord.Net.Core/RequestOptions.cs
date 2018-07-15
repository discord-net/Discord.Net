using System.Threading;

namespace Discord
{
    public class RequestOptions
    {
        public static RequestOptions Default => new RequestOptions();

        /// <summary> 
        /// The max time, in milliseconds, to wait for this request to complete. If null, a request will not time out. 
        /// If a rate limit has been triggered for this request's bucket and will not be unpaused in time, this request will fail immediately. 
        /// </summary>
        public int? Timeout { get; set; }
        public CancellationToken CancelToken { get; set; } = CancellationToken.None;
        public RetryMode? RetryMode { get; set; }
        public bool HeaderOnly { get; internal set; }
        /// <summary>
        /// The reason for this action in the guild's audit log
        /// </summary>
        public string AuditLogReason { get; set; }

        internal bool IgnoreState { get; set; }
        internal string BucketId { get; set; }
        internal bool IsClientBucket { get; set; }
        internal bool IsReactionBucket { get; set; }

        internal static RequestOptions CreateOrClone(RequestOptions options)
        {            
            if (options == null)
                return new RequestOptions();
            else
                return options.Clone();
        }

        public RequestOptions()
        {
            Timeout = DiscordConfig.DefaultRequestTimeout;
        }

        public RequestOptions Clone() => MemberwiseClone() as RequestOptions;
    }
}
