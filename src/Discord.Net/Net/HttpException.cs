using System;
using System.Net;
using System.Runtime.Serialization;

namespace Discord.Net
{
#if NET45
    [Serializable]
#endif
	public class HttpException : Exception
	{
		public HttpStatusCode StatusCode { get; }

		public HttpException(HttpStatusCode statusCode)
			: base($"The server responded with error {(int)statusCode} ({statusCode})")
		{
			StatusCode = statusCode;
        }
#if NET45
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
            => base.GetObjectData(info, context);
#endif
    }
}
