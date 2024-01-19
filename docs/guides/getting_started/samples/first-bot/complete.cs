public class Program
{
	private static DiscordSocketClient _client;
	
	public async Task Main()
	{
		_client = new DiscordSocketClient();
		_client.Log += Log;
		await _client.LoginAsync(TokenType.Bot, 
			Environment.GetEnvironmentVariable("DiscordToken"));
		await _client.StartAsync();
		
		// Block this task until the program is closed.
		await Task.Delay(-1);
	}
	private Task Log(LogMessage msg)
	{
		Console.WriteLine(msg.ToString());
		return Task.CompletedTask;
	}
}