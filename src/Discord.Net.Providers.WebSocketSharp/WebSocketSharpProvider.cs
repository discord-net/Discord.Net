using Discord.Net.WebSockets;

namespace Discord.Providers.WebSocketSharp
{
    public static class WebSocketSharpProvider
    {
        public static readonly WebSocketProvider Instance = () => new WebSocketSharpClient();
    }
}
