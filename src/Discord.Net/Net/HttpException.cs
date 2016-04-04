#pragma warning disable CA1032, CA2237
using System;
using System.Net;

namespace Discord.Net
{
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        
        public HttpException(HttpStatusCode statusCode)
            : base($"The server responded with error {(int)statusCode} ({statusCode})")
        {
            StatusCode = statusCode;
        }
    }
}
