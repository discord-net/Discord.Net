using System;

namespace Discord
{
    public class RpcException : Exception
    {
        public RpcException(int errorCode, string reason = null)
            : base($"The server sent error {errorCode}{(reason != null ? $": \"{reason}\"" : "")}")
        {
            ErrorCode = errorCode;
            Reason = reason;
        }

        public int ErrorCode { get; }
        public string Reason { get; }
    }
}
