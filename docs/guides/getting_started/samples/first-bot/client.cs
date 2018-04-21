private DiscordSocketClient _client;

public async Task MainAsync()
{
	_client = new DiscordSocketClient();

	_client.Log += Log;

	// Remember to keep this private or to read this
	// from an external source!
	string token = "abcdefg...";
	await _client.LoginAsync(TokenType.Bot, token);
	await _client.StartAsync();

	// Block this task until the program is closed.
	await Task.Delay(-1);
}