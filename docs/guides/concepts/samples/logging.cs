using Discord;
using Discord.WebSocket;

public class Program
{
	private DiscordSocketClient _client;
	static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
	
	public async Task MainAsync()
	{
		_client = new DiscordSocketClient(new DiscordSocketConfig
		{
			LogLevel = LogSeverity.Info
		});

		_client.Log += Log;

		await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
		await _client.StartAsync();
		
		await Task.Delay(-1);
	}

	private Task Log(LogMessage message)
	{
		Console.WriteLine(message.ToString());
		return Task.CompletedTask;
	}
}