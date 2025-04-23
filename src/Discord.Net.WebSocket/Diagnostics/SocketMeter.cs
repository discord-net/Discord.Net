#if NET6_0_OR_GREATER
using System.Diagnostics.Metrics;
#endif

namespace Discord.WebSocket.Diagnostics
{
    internal static class SocketMeter
    {
#if NET6_0_OR_GREATER
        private readonly static Meter _meter = new(
            name: "Discord.Net.WebSocket",
            version: typeof(DiscordSocketClient).Assembly.GetName().Version.ToString());
#else
#endif
    }
}
