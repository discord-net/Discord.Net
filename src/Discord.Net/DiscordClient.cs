using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Wumpus;
using Wumpus.Events;

namespace Discord
{
    internal class DiscordClient : IDiscordClient, IDisposable
    {
        public event Action Ready;

        private int _shard, _totalShards;

        public WumpusGatewayClient Gateway { get; }
        public WumpusRestClient Rest { get; }

        public DiscordClient(DiscordClientConfig config)
        {
            _shard = config.Shard;
            _totalShards = config.TotalShards;

            Gateway = new WumpusGatewayClient();
            Rest = new WumpusRestClient();

            var auth = new AuthenticationHeaderValue("", config.Token);
            Gateway.Authorization = auth;
            Rest.Authorization = auth;
        }

        public async Task StartAsync()
        {
            var gateway = await Rest.GetBotGatewayAsync().ConfigureAwait(false);
            await Gateway.RunAsync(gateway.Url.ToString(), _shard, _totalShards).ConfigureAwait(false);
        }

        public async Task StopAsync()
        {
            await Gateway.StopAsync().ConfigureAwait(false);
        }

        private void RegisterEvents()
        {
            Gateway.Ready += OnReady;
        }
        private void OnReady(ReadyEvent args)
        {
            // TODO: Cache
            Ready?.Invoke();
        }

        public void Dispose()
        {
            Gateway.Dispose();
            Rest.Dispose();
        }
    }
}
