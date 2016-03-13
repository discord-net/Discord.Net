using Newtonsoft.Json.Linq;
using System;

namespace Discord.Net.WebSockets
{
    public class WebSocketEventEventArgs : EventArgs
    {
        public string Type { get; }
        public JToken Payload { get; }
    }
}
