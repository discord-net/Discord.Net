using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

public abstract class GatewayEntity<TId> : IGatewayEntity<TId>
    where TId : IEquatable<TId>
{
    public DiscordGatewayClient Client { get; }

    public TId Id { get; }

    internal GatewayEntity(DiscordGatewayClient client, TId id)
    {
        Client = client;
        Id = id;
    }

    IDiscordClient IClientProvider.Client => Client;
}

public interface IGatewayEntity<out TId> :
    IEntity<TId>,
    IGatewayClientProvider
    where TId : IEquatable<TId>;
