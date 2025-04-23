using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Diagnostics
{
    internal static class SocketActivitySource
    {
#if NET5_0_OR_GREATER
        private static readonly ActivitySource _source = new(
            name: "Discord.Net.WebSocket",
            version: typeof(DiscordSocketClient).Assembly.GetName().Version.ToString());
#else
#endif
    }
}
