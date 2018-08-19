using System;

namespace Discord.Net
{
    public class WebSocketClosedException : Exception
    {
        public WebSocketClosedException(int closeCode, string reason = null)
            : base($"The server sent close {closeCode}{(reason != null ? $": \"{reason}\"" : "")}")
        {
            CloseCode = closeCode;
            Reason = reason;
        }

        public int CloseCode { get; }
        public string Reason { get; }
    }
}
