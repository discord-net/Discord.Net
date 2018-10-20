using Discord;
using Discord.WebSocket;

public class Program
{
	private DiscordSocketClient _client;
	static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
	
	public async Task MainAsync()
	{
		// When working with events that have Cacheable<IMessage, ulong> parameters,
		// you must enable the message cache in your config settings if you plan to
		// use the cached message entity. 
		var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
		_client = new DiscordSocketClient(_config);

		await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
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
		// If the message was not in the cache, downloading it will result in getting a copy of `after`.
		var message = await before.GetOrDownloadAsync();
		Console.WriteLine($"{message} -> {after}");
	}
}
