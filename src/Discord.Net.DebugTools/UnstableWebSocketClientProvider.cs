using Discord.Net.WebSockets;

namespace Discord.Net.Providers.UnstableWebSocket
{
    public static class UnstableWebSocketProvider
    {
        public static readonly WebSocketProvider Instance = () => new UnstableWebSocketClient();
    }
}
