using System;

namespace Discord.Net
{
    public class WebSocketException : Exception
    {
        public int Code { get; }
        public string Reason { get; }

        public WebSocketException(int code, string reason)
            : base(GenerateMessage(code, reason))
        {
            Code = code;
            Reason = reason;
        }
        
        private static string GenerateMessage(int? code, string reason)
        {
            if (!String.IsNullOrEmpty(reason))
                return $"Received close code {code}: {reason}";
            else
                return $"Received close code {code}";
        }
    }
}
