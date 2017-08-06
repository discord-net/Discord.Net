using System;
using System.Collections.Generic;
using System.Net;

namespace Discord.Net.Rest
{
    public struct RestResponse
    {
        public HttpStatusCode StatusCode { get; }
        public Dictionary<string, string> Headers { get; }
        public ReadOnlyBuffer<byte> Data { get; }

        public RestResponse(HttpStatusCode statusCode, Dictionary<string, string> headers, ReadOnlyBuffer<byte> data)
        {
            StatusCode = statusCode;
            Headers = headers;
            Data = data;
        }
    }
}
