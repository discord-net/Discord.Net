using System;
using System.Net;

namespace Discord.Net
{
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string Reason { get; }

        public HttpException(HttpStatusCode statusCode, string reason = null)
            : base($"The server responded with error {(int)statusCode} ({statusCode}){(reason != null ? $": \"{reason}\"" : "")}")
        {
            StatusCode = statusCode;
            Reason = reason;
        }
    }
}
