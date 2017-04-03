using Discord;
using Discord.WebSocket;

public class Program
{
	private DiscordSocketClient _client;
	static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
	
	public async Task MainAsync()
	{
		_client = new DiscordSocketClient();

		await _client.LoginAsync(TokenType.Bot, "bot token");
		await _client.StartAsync();

		_client.MessageUpdated += MessageUpdated;
		_client.Ready += () => 
		{
			Console.WriteLine("Bot is connected!");
			return Task.CompletedTask;
		}
		
		await Task.Delay(-1);
	}

	private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
	{
		var message = await before.GetOrDownloadAsync();
		Console.WriteLine($"{message} -> {after}");
	}
}