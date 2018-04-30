private DiscordSocketClient _client;

public async Task MainAsync()
{
	_client = new DiscordSocketClient();

	_client.Log += Log;

	// Remember to keep token private or to read it
	// from an external source!
	await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
	await _client.StartAsync();

	// Block this task until the program is closed.
	await Task.Delay(-1);
}