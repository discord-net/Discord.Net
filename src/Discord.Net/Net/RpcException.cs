using System;

namespace Discord
{
    /// <summary> An exception thrown whenever an RPC error occurs. </summary>
    public class RpcException : Exception
    {
        /// <summary> The code for this error. </summary>
        public int ErrorCode { get; }
        /// <summary> The reason this error occured. </summary>
        public string Reason { get; }

        /// <summary> Creates a new instance of <see cref="RpcException"/> </summary>
        public RpcException(int errorCode, string reason = null)
            : base($"The server sent error {errorCode}{(reason != null ? $": \"{reason}\"" : "")}")
        {
            ErrorCode = errorCode;
            Reason = reason;
        }
    }
}
