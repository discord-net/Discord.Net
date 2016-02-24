using System;

namespace Discord.Net
{
    public class WebSocketException : Exception
    {
        public int Code { get; }
        public string Reason { get; }

        public WebSocketException(int code, string reason) { }
    }
}
