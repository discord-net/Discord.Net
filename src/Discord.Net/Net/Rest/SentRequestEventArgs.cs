namespace Discord.Net.Rest
{
    public class SentRequestEventArgs
    {
        public IRestRequest Request { get; }
        public object Response { get; }
        public double Milliseconds { get; }

        public SentRequestEventArgs(IRestRequest request, object response, double milliseconds)
        {
            Request = request;
            Response = response;
            Milliseconds = milliseconds;
        }
    }
}
