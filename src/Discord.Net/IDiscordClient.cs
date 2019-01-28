using System;
using System.Threading.Tasks;
using Wumpus;

namespace Discord
{
    public interface IDiscordClient
    {
        WumpusGatewayClient Gateway { get; }
        WumpusRestClient Rest { get; }

        Task StartAsync();
        Task StopAsync();

        event Action Ready;
    }
}
