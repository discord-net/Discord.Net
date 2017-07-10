using Discord.API;
using Discord.Logging;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Discord.Rest;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebSocketClient = System.Net.WebSockets.WebSocket;

namespace Discord.Relay
{
    public class RelayServer
    {
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

        private readonly HashSet<RelayConnection> _connections;
        private readonly SemaphoreSlim _lock;
        private readonly JsonSerializer _serializer;
        private readonly DiscordSocketApiClient _discord;
        private int _nextId;

        internal LogManager LogManager { get; }

        internal RelayServer(Action<RelayServer> configAction)
        {
            _connections = new HashSet<RelayConnection>();
            _lock = new SemaphoreSlim(1, 1);
            _serializer = new JsonSerializer();
            _discord = new DiscordSocketApiClient(
                DefaultRestClientProvider.Instance, 
                DefaultWebSocketProvider.Instance,
                DiscordRestConfig.UserAgent);
            configAction?.Invoke(this);

            LogManager = new LogManager(LogSeverity.Debug);
            LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
        }

        internal async Task AcceptAsync(HttpContext context)
        {
            WebSocketClient socket;
            try
            {
                socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
            }
            catch { return; }

            var _ = Task.Run(async () =>
            {
                var conn = new RelayConnection(this, socket, Interlocked.Increment(ref _nextId));
                await AddConnection(conn).ConfigureAwait(false);
                try
                {
                    await conn.RunAsync().ConfigureAwait(false);
                }
                finally { await RemoveConnection(conn).ConfigureAwait(false); }
            });
        }

        internal void StartAsync()
        {
            Task.Run(async () =>
            {
                await _discord.ConnectAsync().ConfigureAwait(false);
            });
        }

        internal async Task AddConnection(RelayConnection conn)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                _connections.Add(conn);
            }
            finally { _lock.Release(); }
        }
        internal async Task RemoveConnection(RelayConnection conn)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                _connections.Remove(conn);
            }
            finally { _lock.Release(); }            
        }

        internal int Serialize(object obj, byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            using (var writer = new StreamWriter(stream))
            {
                _serializer.Serialize(writer, obj);
                return (int)stream.Position;
            }
        }
    }
}
