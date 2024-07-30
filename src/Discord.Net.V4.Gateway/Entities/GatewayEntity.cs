using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

public abstract class GatewayEntity<T> : IEntity<T>
    where T : IEquatable<T>
{
    protected DiscordGatewayClient Client { get; }

    /// <inheritdoc />
    public T Id { get; }

    internal GatewayEntity(DiscordGatewayClient client, T id)
    {
        Client = client;
        Id = id;
    }

    IDiscordClient IClientProvider.Client => Client;
}
