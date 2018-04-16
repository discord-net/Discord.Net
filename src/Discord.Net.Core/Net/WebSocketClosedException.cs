using System;
namespace Discord.Net
{
    /// <summary>
    /// Describes an exception that causes the WebSocket to close during a session.
    /// </summary>
    public class WebSocketClosedException : Exception
    {
        /// <summary>
        ///     Gets the close code sent by Discord.
        /// </summary>
        public int CloseCode { get; }
        /// <summary>
        ///     Gets the reason of the interruption.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebSocketClosedException" /> using the Discord close code
        ///     and the optional reason.
        /// </summary>
        public WebSocketClosedException(int closeCode, string reason = null)
            : base($"The server sent close {closeCode}{(reason != null ? $": \"{reason}\"" : "")}")
        {
            CloseCode = closeCode;
            Reason = reason;
        }
    }
}
