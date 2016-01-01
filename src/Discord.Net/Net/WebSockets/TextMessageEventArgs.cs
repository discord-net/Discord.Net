using System;

namespace Discord.Net.WebSockets
{
    public class TextMessageEventArgs : EventArgs
    {
        public string Message { get; }

        public TextMessageEventArgs(string msg) { Message = msg; }
    }
}
