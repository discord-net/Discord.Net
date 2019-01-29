using System;
using System.Threading.Tasks;
using Wumpus;

namespace Discord
{
    public interface IDiscordClient
    {
        event Action Ready;

        WumpusGatewayClient Gateway { get; }
        WumpusRestClient Rest { get; }

        Task StartAsync();
        Task StopAsync();
    }
}
