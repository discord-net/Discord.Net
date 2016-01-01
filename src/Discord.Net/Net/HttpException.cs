using System;
using System.Net;
using System.Runtime.Serialization;

namespace Discord.Net
{
#if NET46
    [Serializable]
#endif
	public sealed class HttpException : Exception
	{
		public HttpStatusCode StatusCode { get; }

		public HttpException(HttpStatusCode statusCode)
			: base($"The server responded with error {(int)statusCode} ({statusCode})")
		{
			StatusCode = statusCode;
        }
#if NET46
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
            => base.GetObjectData(info, context);
#endif
    }
}
