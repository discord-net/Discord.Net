namespace Discord
{
    /// <summary> Contains options specific to requests </summary>
    public class RequestOptions
    {
        /// <summary> Returns the default options for a request. </summary>
        public static RequestOptions Default => new RequestOptions();

        /// <summary> The max time, in milliseconds, to wait for this request to complete. If null, a request will not time out. If a rate limit has been triggered for this request's bucket and will not be unpaused in time, this request will fail immediately. </summary>
        public int? Timeout { get; set; }

        /// <summary> Creates a new instance of the RequestOptions class </summary>
        public RequestOptions()
        {
            Timeout = 30000;
        }
    }
}
