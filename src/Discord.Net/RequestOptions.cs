namespace Discord
{
    public class RequestOptions
    {
        public static RequestOptions Default => new RequestOptions();

        /// <summary> The max time, in milliseconds, to wait for this request to complete. If null, a request will not time out. If a rate limit has been triggered for this request's bucket and will not be unpaused in time, this request will fail immediately. </summary>
        public int? Timeout { get; set; }

        public RequestOptions()
        {
            Timeout = 30000;
        }
    }
}
