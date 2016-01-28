using Discord.API;
using System;

namespace Discord.Net.Rest
{
    public class RequestEventArgs : EventArgs
    {
        public IRestRequest Request { get; set; }
        public bool Cancel { get; set; }

        public RequestEventArgs(IRestRequest request)
        {
            Request = request;
        }
    }
}
