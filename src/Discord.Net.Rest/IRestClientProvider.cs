namespace Discord.Rest;

public interface IRestClientProvider : IClientProvider
{
    new DiscordRestClient Client { get; }

    IDiscordClient IClientProvider.Client => Client;
}