using Microsoft.AspNetCore.Builder;
using System;

namespace Discord.Relay
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseDiscordRelay(this IApplicationBuilder app, Action<RelayServer> configAction = null)
        {
            var server = new RelayServer(configAction);
            server.StartAsync();
            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                    await server.AcceptAsync(context);
                await next();
            });
        }
    }
}
