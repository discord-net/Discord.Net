using Discord;
using Discord.WebSocket;

public class Program
{
	private DiscordSocketClient _client;
	static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
	
	public async Task MainAsync()
	{
		_client = new DiscordSocketClient();

		await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
		await _client.StartAsync();

		Console.WriteLine("Press any key to exit...");
		Console.ReadKey();

		await _client.StopAsync();
		// Wait a little for the client to finish disconnecting before allowing the program to return
		await Task.Delay(500);
	}
}