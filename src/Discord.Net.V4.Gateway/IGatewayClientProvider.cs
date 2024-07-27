namespace Discord.Gateway;

public interface IGatewayClientProvider : IClientProvider
{
    new DiscordGatewayClient Client { get; }

    IDiscordClient IClientProvider.Client => Client;
}
