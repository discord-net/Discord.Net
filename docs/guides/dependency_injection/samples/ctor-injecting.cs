public class ClientHandler
{
    private readonly DiscordSocketClient _client;

    public ClientHandler(DiscordSocketClient client)
    {
        _client = client;
    }

    public async Task ConfigureAsync()
    {
        //...
    }
}
