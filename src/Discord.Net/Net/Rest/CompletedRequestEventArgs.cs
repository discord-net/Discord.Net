using Discord.API;

namespace Discord.Net.Rest
{
    public class CompletedRequestEventArgs : RequestEventArgs
    {
        public object Response { get; set; }
        public string ResponseJson { get; set; }
        public double Milliseconds { get; set; }

        public CompletedRequestEventArgs(IRestRequest request, object response, string responseJson, double milliseconds)
            : base(request)
        {
            Response = response;
            ResponseJson = responseJson;
            Milliseconds = milliseconds;
        }
    }
}
